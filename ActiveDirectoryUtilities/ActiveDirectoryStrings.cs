using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ActiveDirectoryUtilities
{
  public class ActiveDirectoryStrings
  {
    public static string FMSUDomain = "FUJIMED.COM";
    public static string FMSUDomainIP = "10.25.20.22";
    public static string FMSUZDomain = "FMSUZ.COM";
    public static string FMSUZDomainIP = "10.25.20.151";
    public static string FUJINONDomain = "FUJINON.COM";
    public static string FUJINONDomainIP = "10.198.56.132";
    public static string FFAmericas = "na.ff-americas.com";
    public static string FFAmericasIP = "10.196.115.59";

    private static Dictionary<string, string> _lstDomainStrings = null;
    public static Dictionary<string, string> ListDomainStrings
    {
      get
      {
        if (_lstDomainStrings == null)
        {
          _lstDomainStrings = getListDomainStrings();
        }
        return _lstDomainStrings;
      }
      set
      {
        _lstDomainStrings = value;
      }
    }

    private static Dictionary<string, string> getListDomainStrings()
    {
      Dictionary<string, string> rtrnCode = new Dictionary<string, string>();

      rtrnCode.Add("FMSU Domain", FMSUDomain);
      rtrnCode.Add("FMSU Domain IP", FMSUDomainIP);
      rtrnCode.Add("FMSUZ Domain", FMSUZDomain);
      rtrnCode.Add("FMSUZ Domain IP", FMSUZDomainIP);
      rtrnCode.Add("FUJINON Domain", FUJINONDomain);
      rtrnCode.Add("FUJINON Domain IP", FUJINONDomainIP);
      rtrnCode.Add("'NA' North America Domain", FFAmericas);
      rtrnCode.Add("'NA' Domain IP", FFAmericasIP);

      return rtrnCode;
    }
  }
}
