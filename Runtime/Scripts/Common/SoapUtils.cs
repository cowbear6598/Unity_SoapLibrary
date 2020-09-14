using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

namespace Soap.Tools
{
    public static class SoapUtils
    {
        #region ParticleSystem

        public static void EnableEmission(ParticleSystem particleSystem, bool enabled)
        {
            ParticleSystem.EmissionModule emission = particleSystem.emission;
            emission.enabled = enabled;
        }

        public static float GetEmissionRate(ParticleSystem particleSystem)
        {
            return particleSystem.emission.rateOverTime.constantMax;
        }

        public static void SetEmissionRate(float emissionRate, params ParticleSystem[] particleSystem)
        {
            for (int i = 0; i < particleSystem.Length; i++)
            {
                ParticleSystem.EmissionModule emission = particleSystem[i].emission;
                ParticleSystem.MinMaxCurve rate = emission.rateOverTime.constantMax;
                rate.constantMax = emissionRate;
                emission.rateOverTime = rate;
            }
        }

        #endregion

        #region Hash

        public static string EncodeToSha256(string data)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(data);
            byte[] hash = SHA256Managed.Create().ComputeHash(bytes);

            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < hash.Length; i++)
            {
                builder.Append(hash[i].ToString("X2"));
            }

            return builder.ToString();
        }
        
        public static string EncodeToHMAC_SHA1(string input, byte[] key)
        {
            HMACSHA1 myhmacsha1 = new HMACSHA1(key);
            byte[] byteArray = Encoding.ASCII.GetBytes(input);
            MemoryStream stream = new MemoryStream(byteArray);
            return myhmacsha1.ComputeHash(stream).Aggregate("", (s, e) => s + String.Format("{0:x2}",e), s => s );
        }

        #endregion

        #region UI

        public static void SetCanvasGroup(CanvasGroup[] _canvasGroups, int _index)
        {
            for (int i = 0; i < _canvasGroups.Length; i++)
            {
                _canvasGroups[i].alpha = (i == _index) ? 1 : 0;
                _canvasGroups[i].interactable = (i == _index);
                _canvasGroups[i].blocksRaycasts = (i == _index);
            }
        }

        public static void SetCanvasGroup(CanvasGroup _canvasGroup, bool _IsEnable)
        {
            _canvasGroup.alpha = _IsEnable ? 1 : 0;
            _canvasGroup.interactable = _IsEnable;
            _canvasGroup.blocksRaycasts = _IsEnable;
        }

        public static void SetColorAlpha(Graphic _graphic, float _alpha)
        {
            Color _color = _graphic.color;
            _color.a = _alpha;
            _graphic.color = _color;
        }

        #endregion

        #region 時間計算

        public static DateTime ConvertToLocalDateTime(long _unixTime)
		{
			DateTime _dateTime = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddMilliseconds(Convert.ToDouble(_unixTime));

			return _dateTime.ToLocalTime();
		}
		public static string ConvertToOrderLocalDate(long _unixTime)
		{
			DateTime _dateTime = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddMilliseconds(Convert.ToDouble(_unixTime));
			DateTime _localDateTime = _dateTime.ToLocalTime();

			return _localDateTime.Date.Year + "/" + _localDateTime.Date.Month.ToString("00") + "/" + _localDateTime.Date.Day.ToString("00");
		}
		public static string ConvertToOrderLocalDateTime(long _unixTime)
		{
			DateTime _dateTime = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddMilliseconds(Convert.ToDouble(_unixTime));
			DateTime _localDateTime = _dateTime.ToLocalTime();

			return _localDateTime.Date.Year + "/" + _localDateTime.Date.Month.ToString("00") + "/" + _localDateTime.Date.Day.ToString("00") + " " + _localDateTime.Hour.ToString("00") + ":" + _localDateTime.Minute.ToString("00") + ":" +
			       _localDateTime.Second.ToString("00");
		}
		public static string ConvertToLocalDateTime(long _unixTime, int _day)
		{
			DateTime _dateTime = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).AddMilliseconds(Convert.ToDouble(_unixTime));
			DateTime _localDateTime = _dateTime.ToLocalTime();

			string _weekDay = "";

			switch (_day)
			{
				case 1:
					_weekDay = "星期一";
					break;
				case 2:
					_weekDay = "星期二";
					break;
				case 3:
					_weekDay = "星期三";
					break;
				case 4:
					_weekDay = "星期四";
					break;
				case 5:
					_weekDay = "星期五";
					break;
				case 6:
					_weekDay = "星期六";
					break;
				case 0:
					_weekDay = "星期日";
					break;
			}

			return _localDateTime.Date.Year + "/" + _localDateTime.Date.Month.ToString("00") + "/" + _localDateTime.Date.Day.ToString("00") + " " + _weekDay;
		}
		public static double ConvertToUnixTime(DateTime _dateTime)
		{
			DateTime origin = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc).AddMilliseconds(_dateTime.Millisecond);
			TimeSpan diff = (_dateTime.ToUniversalTime() - origin);
			return Math.Floor(diff.TotalMilliseconds);
		}

        #endregion

        public static string CalculateTime(float _second)
        {
            float milliSecond = (int) ((_second % 1) * 100);
            float minute = (int) (_second / 60f);

            return Mathf.Clamp(minute, 0, 99).ToString("00") + ":" + ((int) (_second % 60)).ToString("00") + ":" + Mathf.Clamp(milliSecond, 0, 99).ToString("00");
        }

//    public static void StartCinemachineCameraShake(CinemachineBasicMultiChannelPerlin _cameraNoise, float _fadeSpeed = 1f, float _power = 0.35f, float _frequency = 2f)
//    {
//        if(_cameraNoise == null) return;
//
//        Timing.RunCoroutine(CinemachineCameraShake(_cameraNoise,_fadeSpeed,_power,_frequency));
//    }
//
//    private static IEnumerator<float> CinemachineCameraShake(CinemachineBasicMultiChannelPerlin _cameraNoise,float _fadeSpeed, float _power, float _frequency)
//    { 
//        
//        
//        _cameraNoise.m_AmplitudeGain = _power;
//        _cameraNoise.m_FrequencyGain = _frequency;
//        
//        while (_cameraNoise.m_AmplitudeGain > 0 || _cameraNoise.m_FrequencyGain > 0)
//        {
//            _cameraNoise.m_AmplitudeGain = Mathf.MoveTowards(_cameraNoise.m_AmplitudeGain, 0, Time.deltaTime * _fadeSpeed);
//            _cameraNoise.m_FrequencyGain = Mathf.MoveTowards(_cameraNoise.m_FrequencyGain, 0, Time.deltaTime * _fadeSpeed);
//            
//            yield return Timing.WaitForOneFrame;
//        }
//    }
    }
}