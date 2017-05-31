using System;
using System.Collections.Generic;
using System.DirectoryServices;
using System.DirectoryServices.AccountManagement;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ActiveDirectoryUtilities
{
  public class AD_Utility
  {
    #region public properties surfaced by this class

    private string _errorMessage = string.Empty;
    public string ErrorMessage
    {
      get { return _errorMessage; }
      set { _errorMessage = value; }
    }

    private Exception _operationException = null;
    public Exception OperationException
    {
      get { return _operationException; }
      set
      {
        _operationException = value;
        ErrorMessage = _operationException.Message;
      }
    }

    private void resetErrorProperties()
    {
      _errorMessage = string.Empty;
      _operationException = null;
    }

    #endregion public properties surfaced by this class
    public bool AuthenticateUser(string domain, string userName, string password)
    {
      bool rtrnCode = false;

      using (var context = new PrincipalContext(ContextType.Domain, domain, string.Format("{0}\\{1}", domain, userName), password))
      {
        //Username and password for authentication.
        rtrnCode = context.ValidateCredentials(userName, password);
      }

      #region REMMED OUT DirectoryEntry

      /*
                  DirectoryEntry dirEntry = null;
                  object nativeObject = null;

                  try
                  {
                      // At this point no errors have been encountered so clear the error related properties.
                      resetErrorProperties();

                      dirEntry = GetDirectoryEntry(domain, userName, password);
                      if ((dirEntry != null) &&
                          (dirEntry.Guid != null))
                      {
                          nativeObject = dirEntry.NativeObject;
                          rtrnCode = true;
                      }
                  }
                  catch (DirectoryServicesCOMException dsEx)
                  {
                      dsEx.Source = "AuthenticateUser Exception";
                      OperationException = dsEx;
                      throw dsEx;
                  }
                  finally
                  {
                      nativeObject = null;
                      dirEntry = null;
                  }
      */

      #endregion REMMED OUT DirectoryEntry

      return rtrnCode;
    }

    public bool DoesUserExist(string domain, string userName)
    {
      bool rtrnCode = false;
      using (var domainContext = new PrincipalContext(ContextType.Domain, domain))
      {
        using (UserPrincipal foundUser = UserPrincipal.FindByIdentity(domainContext, IdentityType.SamAccountName, userName))
        {
          rtrnCode = foundUser != null;
        }
      }

      return rtrnCode;
    }

    public Guid? GetUserGuid(string userName)
    {
      Guid? rtrnCode = null;
      using (PrincipalContext ctx = new PrincipalContext(ContextType.Domain))
      {
        // Find a User
        UserPrincipal user = UserPrincipal.FindByIdentity(ctx, userName);

        if (user != null)
        {
          rtrnCode = user.Guid;
        }
      }

      return rtrnCode;
    }

    public DirectoryEntry GetDirectoryEntry(string domain)
    {
      return new DirectoryEntry(string.Format("LDAP://{0}", domain));
    }

    public DirectoryEntry GetDirectoryEntry(string domain, string userName, string password)
    {
      return new DirectoryEntry(string.Format("LDAP://{0}, {1}, {2}", domain, userName, password));
    }

    public UserPrincipal GetUserPrincipal(string userName)
    {
      UserPrincipal rtrnCode = null;

      try
      {
        using (PrincipalContext ctx = new PrincipalContext(ContextType.Domain))
        {
          rtrnCode = UserPrincipal.FindByIdentity(ctx, userName);
        }
      }
      catch (Exception ex)
      {
        ErrorMessage = ex.Message;
      }

      return rtrnCode;
    }

    public UserPrincipal GetUserPrincipal(string domain, string userName)
    {
      UserPrincipal rtrnCode = null;

      try
      {
        using (var domainContext = new PrincipalContext(ContextType.Domain, domain))
        {
          rtrnCode = UserPrincipal.FindByIdentity(domainContext, IdentityType.SamAccountName, userName);
        }
      }
      catch (Exception ex)
      {
        ErrorMessage = ex.Message;
      }

      return rtrnCode;
    }

    public bool HasUserPasswordExpired(string domain, UserPrincipal user)
    {
      bool rtrnCode = false;
      DirectoryEntry objDE = null;
      DirectorySearcher mySrchr = null;
      SearchResultCollection sResults = null;
      string filter = "maxPwdAge=*";
      long maxDays = 0;
      Int64 maxPwdAge = -1;

      try
      {
        objDE = user.GetUnderlyingObject() as DirectoryEntry;
        mySrchr = new DirectorySearcher(objDE);
        mySrchr.Filter = filter;

        sResults = mySrchr.FindAll();
        if (sResults.Count >= 1)
        {
          maxPwdAge = (Int64)sResults[0].Properties["maxPwdAge"][0];
          maxDays = maxPwdAge / -864000000000;
        }
      }
      catch (Exception ex)
      {
        ErrorMessage = ex.Message;
      }
      finally
      {
        sResults = null;
        mySrchr = null;
        objDE = null;
      }

      return rtrnCode;
    }

    public bool HasUserPasswordExpiredSecondDraft(string domain, UserPrincipal user)
    {
      bool rtrnCode = false;

      string domainStr = string.Empty;
      string domainAndUsername = string.Empty;
      AuthenticationTypes at = AuthenticationTypes.Secure;
      StringBuilder sb = null;
      string principle = string.Empty;
      string pw = string.Empty;
      DirectoryEntry objDE = null;
      DirectorySearcher mySrchr = null;
      SearchResultCollection sResults = null;
      string filter = "maxPwdAge=*";
      long maxDays = 0;
      Int64 maxPwdAge = -1;

      try
      {
        sb = new StringBuilder();

        switch (domain.ToLower())
        {
          case "fmsu":
            domainStr = @"LDAP://fujimed.com";
            domainAndUsername = @"LDAP://fujimed.com/" + string.Format("CN={0},CN=User,OU=USERS,dc=fujimed,dc=com", user.SamAccountName);
            principle = "etqread";
            pw = "e3qNfuj1";
            break;
        }

        objDE = new DirectoryEntry(domain, principle, pw, at);
        if (objDE != null)
        {
          mySrchr = new DirectorySearcher(objDE);
          mySrchr.Filter = filter;

          sResults = mySrchr.FindAll();
          if (sResults.Count>=1)
          {
            maxPwdAge = (Int64)sResults[0].Properties["maxPwdAge"][0];
            maxDays = maxPwdAge / -864000000000;
          }
        }
      }
      catch (Exception ex)
      {
        string msg = ex.Message;
      }
      finally
      {
        sResults = null;
        mySrchr = null;
        objDE = null;
        sb = null;
      }

      return rtrnCode;
    }

    public bool HasUserPasswordExpiredFirstDraft(string domain, UserPrincipal user)
    {
      bool rtrnCode = false;
      string domainStr = string.Empty;
      string principle = string.Empty;
      string pw = string.Empty;
      AuthenticationTypes at = AuthenticationTypes.Secure;

      switch (domain.ToLower())
      {
        case "fmsu":
          domainStr = string.Format("fujimed.com/CN={0},CN=User,OU=USERS,dc=fujimed,dc=com", user.SamAccountName);  // "LDAP://10.25.20.22:389/CN={UserName},CN=Users,dc=fujimed,dc=com"
                                                                                                                    //domainStr = string.Format("10.25.20.22:389/CN={0},CN=User,OU=USERS,dc=fujimed,dc=com", user.SamAccountName);  // "LDAP://10.25.20.22:389/CN={UserName},CN=Users,dc=fujimed,dc=com"
          principle = "etqread";
          pw = "e3qNfuj1";
          break;
        case "fmsuz":
          //domainStr = string.Format("10.41.20.1:389/CN=Users,dc=fmsuz,dc=com");
          domainStr = string.Format("fmsuz.com/CN=Users,dc=fmsuz,dc=com");
          break;
        case "fujinon-wnj":
          domainStr = string.Format("10.198.53.132:389/CN=Users,dc=FUJINON,dc=com");
          break;
        case "na":
          domainStr = string.Format("10.196.115.59:389/CN=Users,dc=na.ff-americas,dc=com");
          break;
      }

      DirectoryEntry objDE = null;

      //objDE = new DirectoryEntry(string.Format("LDAP://{0}", domainStr), principle, pw, at);
      objDE = new DirectoryEntry(string.Format("LDAP://{0}", domainStr));
      //objDE = new DirectoryEntry(@"LDAP://10.25.20.22:389/CN=DonWa,CN=Users,dc=fujimed,dc=com");

      DirectoryEntries objChild = objDE.Children;

      /*
                  //using (var userEntry = new System.DirectoryServices.DirectoryEntry(string.Format("WinNT://{0}/{1},user", domain, user.SamAccountName)))
                  using (var userEntry = new System.DirectoryServices.DirectoryEntry(string.Format("LDAP://{0}/{1},user", domainStr, user.SamAccountName)))
                  {
                      var maxPasswordAge = (int)userEntry.Properties.Cast<System.DirectoryServices.PropertyValueCollection>().First(p => p.PropertyName == "MaxPasswordAge").Value;
                      var passwordAge = (int)userEntry.Properties.Cast<System.DirectoryServices.PropertyValueCollection>().First(p => p.PropertyName == "PasswordAge").Value;
                      if (user.LastPasswordSet != null)
                      {
                          rtrnCode = (passwordAge > maxPasswordAge);
                      }
                  }
      */
      return rtrnCode;
    }

    public bool IsAccountLocked(string domain, string userName)
    {
      bool rtrnCode = true;
      UserPrincipal user = null;

      try
      {
        user = GetUserPrincipal(domain, userName);

        if (user != null)
        {
          rtrnCode = user.IsAccountLockedOut();
        }
      }
      catch (Exception ex)
      {
        this.ErrorMessage = ex.Message;
      }
      finally
      {
        user = null;
      }

      return rtrnCode;
    }
  }
}
