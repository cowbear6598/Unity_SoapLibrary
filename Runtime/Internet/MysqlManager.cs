using System;
using System.Collections.Generic;
using System.Text;
using MEC;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Networking;

namespace Soap.Internet
{
    public static class MysqlManager
    {
        public delegate void MysqlManagerCallback();
        public static MysqlManagerCallback OnConnectFail;
        
        //Url Settings
        private static string url = "http://127.0.0.1:8080/";
        private static string urlSSL = "";

        #region Reconnect
        
        private enum ReconnectState
        {
            RequestAPIByGet = 0,
            RequestAPIByGet2 = 1,
            RunRequestAPIByPost = 2,
            RunRequestAPIByPost2 = 3
        }
        private static ReconnectState reconnectState = ReconnectState.RequestAPIByGet;
        
        private static Action<string> nowAction;
        private static string nowApi;
        private static bool nowSecure;
        private static string nowToken;
        private static string[] nowKey;
        private static object nowData;
        
        #endregion
        
        #region GET

        public static void RunRequestAPIByGet(Action<string> _action, string _api, bool _secure, params string[] _key)
        {
            Timing.RunCoroutine(RequestAPIByGet(_action, _api, _secure, _key));
        }
        public static void RunRequestAPIByGet(Action<string> _action, string _api, string _token, bool _secure, params string[] _key)
        {
            Timing.RunCoroutine(RequestAPIByGet(_action, _api, _token, _secure, _key));
        }

        private static IEnumerator<float> RequestAPIByGet(Action<string> _action, string _api, bool _secure, params string[] _key)
        {
            nowAction = _action;
            nowApi = _api;
            nowSecure = _secure;
            nowKey = _key;

            reconnectState = ReconnectState.RequestAPIByGet;

            using (UnityWebRequest req = UnityWebRequest.Get(GetAPIUrl(_secure, _api, _key)))
            {
                yield return Timing.WaitUntilDone(req.SendWebRequest());

                CallbackMessage(_action, req);
            }
        }
        private static IEnumerator<float> RequestAPIByGet(Action<string> _action, string _api, string _token, bool _secure, params string[] _key)
        {
            nowAction = _action;
            nowApi = _api;
            nowToken = _token;
            nowSecure = _secure;
            nowKey = _key;

            reconnectState = ReconnectState.RequestAPIByGet2;

            using (UnityWebRequest req = UnityWebRequest.Get(GetAPIUrl(_secure, _api, _key)))
            {
                req.SetRequestHeader("Authorization", _token);

                yield return Timing.WaitUntilDone(req.SendWebRequest());

                CallbackMessage(_action, req);
            }
        }

        #endregion

        #region POST

        public static void RunRequestAPIByPost(Action<string> _action, string _api, object _data, bool _secure,params string[] _key)
        {
            Timing.RunCoroutine(RequestAPIByPost(_action, _api, _data, _secure, _key));
        }
        public static void RunRequestAPIByPost(Action<string> _action, string _api,string _token, object _data, bool _secure,params string[] _key)
        {
            Timing.RunCoroutine(RequestAPIByPost(_action, _api, _token, _data, _secure, _key));
        }
        
        private static IEnumerator<float> RequestAPIByPost(Action<string> _action, string _api, object _data, bool _secure, params string[] _key)
        {
            nowAction = _action;
            nowApi = _api;
            nowData = _data;
            nowSecure = _secure;
            nowKey = _key;

            reconnectState = ReconnectState.RunRequestAPIByPost;

            using (UnityWebRequest req = new UnityWebRequest(GetAPIUrl(_secure, _api,_key), UnityWebRequest.kHttpVerbPOST))
            {
                req.uploadHandler = CreateJsonUploadHandler(_data);

                req.downloadHandler = new DownloadHandlerBuffer();

                req.SetRequestHeader("Content-Type", "application/json");

                yield return Timing.WaitUntilDone(req.SendWebRequest());

                CallbackMessage(_action, req);
            }
        }        
        private static IEnumerator<float> RequestAPIByPost(Action<string> _action, string _api, string _token,object _data, bool _secure, params string[] _key)
        {
            nowAction = _action;
            nowApi = _api;
            nowData = _data;
            nowSecure = _secure;
            nowToken = _token;
            nowKey = _key;

            reconnectState = ReconnectState.RunRequestAPIByPost2;

            using (UnityWebRequest req = new UnityWebRequest(GetAPIUrl(_secure, _api,_key), UnityWebRequest.kHttpVerbPOST))
            {
                req.uploadHandler = CreateJsonUploadHandler(_data);

                req.downloadHandler = new DownloadHandlerBuffer();
            
                req.SetRequestHeader("Authorization", _token);
                req.SetRequestHeader("Content-Type", "application/json");

                yield return Timing.WaitUntilDone(req.SendWebRequest());

                CallbackMessage(_action, req);
            }
        }
        
        #endregion

        private static string GetAPIUrl(bool _secure, string _api, params string[] _key)
        {
            string _finalUrl = ((_secure) ? urlSSL : url) + _api + "?";

            for (int i = 0; i < _key.Length; i++)
            {
                _finalUrl += (i == 0) ? _key[i] : "&" + _key[i];
            }

            Debug.Log(_finalUrl);

            return _finalUrl;
        }

        private static void CallbackMessage(Action<string> _action, UnityWebRequest _req)
        {
            Debug.Log(""+_req.responseCode + " :" +_req.downloadHandler.text);

            switch (_req.responseCode)
            {
                case 200:
                    _action?.Invoke(_req.downloadHandler.text);
                    break;
                case 204:
                    _action?.Invoke(_req.downloadHandler.text);
                    break;
                default:
                    Debug.Log(_req.responseCode);
                    OnConnectFail?.Invoke();
                    break;
            }
        }

        private static UploadHandler CreateJsonUploadHandler(object _data, params string[] _key)
        {
            if ((_data == null && _key.Length == 0) || (_data != null && _key.Length != 0)) return null;

            byte[] _jsonRaw = null;

            if (_data != null)
            {
                _jsonRaw = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(_data));
            }
            else if (_key.Length > 0)
            {
                string _combineData = "";

                for (int i = 0; i < _key.Length; i++)
                {
                    _combineData += (i == 0) ? _key[i] : "&" + _key[i];
                }

                _jsonRaw = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(_combineData));
            }

            UploadHandler _uploadHandler = new UploadHandlerRaw(_jsonRaw);
            _uploadHandler.contentType = "application/json";

            return _uploadHandler;
        }

        public static void Reconnect()
        {
            switch (reconnectState)
            {
                case ReconnectState.RequestAPIByGet:
                    RunRequestAPIByGet(nowAction, nowApi, nowSecure, nowKey);
                    break;
                case ReconnectState.RequestAPIByGet2:
                    RunRequestAPIByGet(nowAction, nowApi, nowToken, nowSecure, nowKey);
                    break;
                case ReconnectState.RunRequestAPIByPost:
                    RunRequestAPIByPost(nowAction, nowApi, nowData, nowSecure, nowKey);
                    break;
                case ReconnectState.RunRequestAPIByPost2:
                    RunRequestAPIByPost(nowAction, nowApi, nowToken, nowData, nowSecure, nowKey);
                    break;
            }
        }
    }
}

// 自訂一伺服器回傳
public class S2C_ResponseData
{
    public int status;
    public string message;
    public string data;
}
