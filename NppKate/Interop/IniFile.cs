// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Text;

namespace NppKate
{
    public class IniFile
    {
        [DllImport("kernel32")]
        static extern int GetPrivateProfileString(string section, string key, string defaultValue, StringBuilder retval, int size, string file);

        [DllImport("kernel32")]
        static extern long WritePrivateProfileString(string section, string key, string value, string file);

        protected string file;

        public IniFile()
        {
        }

        public IniFile(string file)
        {
            this.file = file;
        }

        public void SetValue<T>(string section, string key, T value)
        {
            try
            {
                WritePrivateProfileString(section, key.Trim(), value.ToString().Trim(), file);
            }
            catch { }
        }

        public T GetValue<T>(string section, string key, T defaultValue, int size = 255*10) //MAX_PATH*10
        {
            try
            {
                var retval = new StringBuilder(size);

                GetPrivateProfileString(section, key, defaultValue.ToString(), retval, size, file);
                return (T)TypeDescriptor.GetConverter(typeof(T)).ConvertFrom(retval.ToString());
            }
            catch
            {
                return defaultValue;
            }
        }
    }

}