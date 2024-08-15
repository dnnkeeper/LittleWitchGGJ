using System;
using UnityEngine;
using UnityEngine.Events;

namespace SharedAssets.Utility_Scripts
{
    public class PlayerPrefsStringSaver : MonoBehaviour
    {
        public string PrefName => preferenceName;
        [SerializeField] string preferenceName = "";
        public string RestoredValue => restoredValue;
        string restoredValue;

        public UnityEvent<string> onLoad;

        private void Awake()
        {
            RestoreSavedString();
        }

        public void SetPrefName(string s)
        {
            preferenceName = s;
        }

        public void LoadString()
        {
            RestoreSavedString();
        }

        public bool IsKeyExists()
        {
            return PlayerPrefs.HasKey(preferenceName);
        }

        public string RestoreSavedString()
        {
            restoredValue = String.Empty;
            if (IsKeyExists())
            {
                restoredValue = PlayerPrefs.GetString(preferenceName);
                Debug.Log("Restored '" + preferenceName + "': '" + restoredValue + "'");
                onLoad.Invoke(restoredValue);
            }
            return restoredValue;
        }

        public void SaveString(string s)
        {
            if (string.IsNullOrEmpty(preferenceName))
            {
                var t = transform;
                string path = t.name;
                while (t.parent != null)
                {
                    t = transform.parent;
                    path = t.name + "/" + path;
                }
                preferenceName = path;
            }
            Debug.Log($"Saved {preferenceName}={s}");
            PlayerPrefs.SetString(preferenceName, s);
        }

        public void ClearPref()
        {
            if (IsKeyExists())
                PlayerPrefs.DeleteKey(preferenceName);
        }
    }
}
