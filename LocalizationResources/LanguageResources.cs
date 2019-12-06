using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Serialization.Json;
using System.Text;
using System.Windows;

/// <summary>
/// 다국어 지원
/// https://developerfeel.blogspot.com/2014/07/wpf-globalization-language-using.html
/// </summary>
namespace LocalizationResources
{
    public sealed class LanguageResources : INotifyPropertyChanged
    {
        #region Singleton instance
        private static volatile LanguageResources instance;
        private static object syncRoot = new Object();
                
        private LanguageResources()
        {
            LoadResource();
        }
        ~LanguageResources()
        {
            if (ResourceDictionary != null)
            {
                ResourceDictionary.Clear();
                ResourceDictionary = null;
            }

            if (NotifyPropertyChangedDictoionary != null)
            {
                NotifyPropertyChangedDictoionary.Clear();
                NotifyPropertyChangedDictoionary = null;
            }
        }

        public static LanguageResources Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (syncRoot)
                    {
                        if (instance == null)
                        {
                            instance = new LanguageResources();
                        }
                    }
                }

                return instance;
            }
        }
        #endregion
        
        #region LoadResource
        private void LoadResource()
        {
            string fileStream = "";
            try
            {
                // JSON 언어 파일
                //string assemblyFolder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().CodeBase).Substring(6);
                //string filepath = string.Format(Settings.LANGUAGE_FILE_PATH, assemblyFolder, CultureName);
                //fileStream = File.ReadAllText(filepath);

                //DataContractJsonSerializer dcjs = new DataContractJsonSerializer(typeof(Dictionary<string, string>));
                //byte[] fileByte = Encoding.UTF8.GetBytes(fileStream);
                //MemoryStream ms = new MemoryStream(fileByte);

                // CSV 언어 파일
                Dictionary<string, string> tempDictionary = new Dictionary<string, string>();
                string assemblyFolder = Path.GetDirectoryName(Assembly.GetExecutingAssembly().CodeBase).Substring(6);
                string filepath = string.Format("{0}\\Resource\\language.tsv", assemblyFolder);
                //fileStream = File.ReadAllText(filepath);
                int languageIdx = 0;
                //string[] lines = fileStream.Split('\n');
                string[] lines = File.ReadAllLines(filepath);
                for (int i = 0; i < lines.Length; i++)
                {
                    if (string.IsNullOrEmpty(lines[i]))
                        continue;
                    string[] keyValues = lines[i].Split('\t');
                    if (i == 0) // 언어 인덱스
                    {
                        for (int j = 0; j < lines.Length; j++)
                        {
                            if (keyValues[j] == CultureName)
                            {
                                languageIdx = j;
                                break;
                            }
                        }
                    }
                    else
                    {
                        if (keyValues.Length >= languageIdx && !string.IsNullOrEmpty(keyValues[languageIdx]))
                            tempDictionary.Add(keyValues[0], keyValues[languageIdx]);
                        else if (keyValues.Length >= 2 && !string.IsNullOrEmpty(keyValues[1]))
                            tempDictionary.Add(keyValues[0], keyValues[1]);
                        else
                            tempDictionary.Add(keyValues[0], keyValues[0]);
                    }
                }

                if (ResourceDictionary != null)
                {
                    ResourceDictionary.Clear();
                    ResourceDictionary = null;
                }
                //ResourceDictionary = dcjs.ReadObject(ms) as Dictionary<string, string>;
                ResourceDictionary = tempDictionary;
            }
            catch
            {
                Debug.Assert(false);
            }
        }
        #endregion

        #region ResourceDictionary
        private Dictionary<string, string> _resourceDictionary;
        public Dictionary<string, string> ResourceDictionary
        {
            set
            {
                _resourceDictionary = value;
                if (_resourceDictionary != null && PropertyChanged != null)
                {
                    // Set property name "Binding.IndexerName" for PropertyChanged event
                    PropertyChanged(this, new PropertyChangedEventArgs("Item[]"));
                    // call PropertyChanged in registered viewmodels implement INotifyPropertyChanged interface
                    foreach (var item in NotifyPropertyChangedDictoionary)
                    {
                        if (item.Key != null && item.Value != null)
                        {
                            foreach (string propertyname in item.Value)
                            {
                                PropertyChanged(item.Key, new PropertyChangedEventArgs(propertyname));
                            }
                        }
                    }
                }
            }
            get
            {
                return _resourceDictionary;
            }
        }
        #endregion

        #region Indexer
        public string this[string key]
        {
            get
            {
                string value = key == null ? "" : key;

                if (ResourceDictionary != null && ResourceDictionary.ContainsKey(key) == true)
                {
                    value = ResourceDictionary[key];
                }
                else
                {
                    value = key;
                }
                return value;
            }
        }
        #endregion
                
        #region CultureName
        /// <summary>
        /// To load resources, set CultureName. Default is "ko-KR"
        /// </summary>
        public string CultureName
        {
            get
            {
                return _CultureName;
            }
            set
            {
                CultureInfo ci = CultureInfo.GetCultures(CultureTypes.AllCultures).FirstOrDefault(f => f.Name == value);
                if (ci != null)
                {
                    _CultureName = value;
                    LoadResource();
                }
            }
        }
        //private string _CultureName = Thread.CurrentThread.CurrentCulture.Name;
        private string _CultureName = "ko-KR";
        #endregion

        #region NotifyPropertyChangedDictoionary
        private Dictionary<INotifyPropertyChanged, string[]> _NotifyPropertyChangedDictoionary = null;
        private Dictionary<INotifyPropertyChanged, string[]> NotifyPropertyChangedDictoionary
        {
            get
            {
                if (_NotifyPropertyChangedDictoionary == null)
                {
                    _NotifyPropertyChangedDictoionary = new Dictionary<INotifyPropertyChanged, string[]>();
                }
                return _NotifyPropertyChangedDictoionary;
            }
            set
            {
                _NotifyPropertyChangedDictoionary = value;
            }
        }
        #endregion

        #region SetRegisterNotifyPropertyChanged
        public void SetRegisterNotifyPropertyChanged(INotifyPropertyChanged sender, params string[] propertynames)
        {
            if (NotifyPropertyChangedDictoionary != null)
            {
                NotifyPropertyChangedDictoionary.Add(sender, propertynames);
            }
        }
        #endregion

        #region INotifyPropertyChanged, PropertyChanged
        public event PropertyChangedEventHandler PropertyChanged;
        #endregion
    }
}