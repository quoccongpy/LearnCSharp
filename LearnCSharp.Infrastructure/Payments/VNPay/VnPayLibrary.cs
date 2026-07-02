using System.Net;
using System.Text;

// https://www.youtube.com/watch?v=GEnt7jmIUiI&t=135s
namespace LearnCSharp.Infrastructure.Payments.VNPay
{
    public class VnPayLibrary
    {
        private readonly SortedList<string, string> _requestData = new SortedList<string, string>(new VnPayCompare());
        private readonly SortedList<string, string> _responseData = new SortedList<string, string>(new VnPayCompare());

        public void AddRequestData(string key, string value)
        {
            if (!string.IsNullOrEmpty(value))
            {
                _requestData.Add(key, value);
            }
        }
        public void AddResponseData(string key, string value)
        {
            if (!string.IsNullOrEmpty(value))
            {
                _responseData.Add(key, value);
            }
        }

        public string GetResponseData(string key)
        {
            return _responseData.TryGetValue(key, out var retValue) ? retValue : string.Empty;
        }

        public string CreateRequestUrl(string baseUrl, string vnpHashSecret)
        {
            var data = new StringBuilder();

            foreach (var (key, value) in _requestData.Where(a => !string.IsNullOrEmpty(a.Value)))
            {
                data.Append(WebUtility.UrlEncode(key) + "=" + WebUtility.UrlEncode(value) + "&");
            }
            var queryString = data.ToString();
            baseUrl += "?" + queryString;
            var signData = queryString;
            if (signData.Length > 0)
            {
                signData = signData.Remove(data.Length - 1, 1);
            }
            var vnpSecureHash = KeySecurity.HmacSHA512(vnpHashSecret, signData);
            baseUrl += "vnp_SecureHash=" + vnpSecureHash;
            return baseUrl;
        }
        public bool ValidateSignature(string inputHash, string secretKey)
        {
            var rspRaw = GetResponseData();
            var myCheckSum = KeySecurity.HmacSHA512(secretKey, rspRaw);
            return myCheckSum.Equals(inputHash, StringComparison.InvariantCultureIgnoreCase);
        }

        private string GetResponseData()
        {
            var data = new StringBuilder();
            if (_responseData.ContainsKey("vnp_SecureHashType"))
            {
                _responseData.Remove("vnp_SecureHashType");
            }

            if (_responseData.ContainsKey("vnp_SecureHash"))
            {
                _responseData.Remove("vnp_SecureHash");
            }

            foreach (var (key, value) in _responseData.Where(a => !string.IsNullOrEmpty(a.Value)))
            {
                data.Append(WebUtility.UrlEncode(key) + "=" + WebUtility.UrlEncode(value) + "&");
            }

            if (data.Length > 0)
            {
                data.Remove(data.Length - 1, 1);
            }

            return data.ToString();
        }

    }
}