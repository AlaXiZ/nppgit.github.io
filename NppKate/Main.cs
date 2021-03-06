// This is an open source non-commercial project. Dear PVS-Studio, please check it.
// PVS-Studio Static Code Analyzer for C, C++ and C#: http://www.viva64.com
/*
Copyright (c) 2015-2016, Schadin Alexey (schadin@gmail.com)
All rights reserved.

Redistribution and use in source and binary forms, with or without modification, are permitted 
provided that the following conditions are met:

1. Redistributions of source code must retain the above copyright notice, this list of conditions 
and the following disclaimer.

2. Redistributions in binary form must reproduce the above copyright notice, this list of conditions 
and the following disclaimer in the documentation and/or other materials provided with 
the distribution.

3. Neither the name of the copyright holder nor the names of its contributors may be used to endorse 
or promote products derived from this software without specific prior written permission.

THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS "AS IS" AND ANY EXPRESS OR 
IMPLIED WARRANTIES, INCLUDING, BUT NOT LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND 
FITNESS FOR A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT HOLDER OR 
CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL, EXEMPLARY, OR CONSEQUENTIAL 
DAMAGES (INCLUDING, BUT NOT LIMITED TO, PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, 
DATA, OR PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF LIABILITY, WHETHER 
IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF 
THE USE OF THIS SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
*/

using NLog;
using NppKate.Common;
using NppKate.Modules.GitCore;
using NppKate.Npp;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace NppKate
{
    class Main
    {
        #region "Fields"
        private static CommandManager cm = new CommandManager();
        private static ModuleManager mm = new ModuleManager(cm, new FormManager());
        private static readonly IList<Type> _excludedTypes = new ReadOnlyCollection<Type>(
            new List<Type> {
                typeof(GitRepository)
            });
        private static Logger _logger;
        #endregion

        #region "Main"
        public static void Init()
        {
            _logger = LogManager.GetCurrentClassLogger();

            LoadModules();
            try
            {
                mm.AddModule(GitRepository.Module); // TODO: ���������� �� �������������� ��������
            }
            catch (Exception ex)
            {
                LoggerUtil.Error(_logger, ex, "mm.AddModule(GitRepository.Module)", null);
            }

            try
            {
                mm.Init();
            }
            catch (Exception ex)
            {
                LoggerUtil.Error(_logger, ex, "Init", null);
            }

            NppInfo.Instance.AddCommand("Restart Notepad++", NppUtils.Restart);
            NppInfo.Instance.AddCommand("Settings", DoSettings);
            NppInfo.Instance.AddCommand("About", DoAbout);
        }

        public static void ToolBarInit()
        {
            try
            {
                mm.ToolBarInit();
            }
            catch (Exception ex)
            {
                LoggerUtil.Error(_logger, ex, "ToolBarInit", null);
            }
        }

        public static void PluginCleanUp()
        {
            try
            {
                mm.Final();
            }
            catch (Exception ex)
            {
                LoggerUtil.Error(_logger, ex, "PluginCleanUp", null);
            }
        }

        public static void MessageProc(SCNotification sn)
        {
            try
            {
                mm.MessageProc(sn);
            }
            catch (Exception ex)
            {
                LoggerUtil.Error(_logger, ex, "MessageProc NppMsg={0} or SciMsg={1}", (NppMsg)sn.nmhdr.code, (SciMsg)sn.message);
            }
        }
        #endregion

        #region "Utils"
        private static void LoadModules()
        {
            var imodule = typeof(IModule);
            var types = AppDomain.CurrentDomain.GetAssemblies()
                        .SelectMany(s => s.GetTypes())
                        .Where(p => imodule.IsAssignableFrom(p))
                        .Where(p => p.IsClass)                    // ����� ������ ������
                        .Where(p => !_excludedTypes.Contains(p))  // �������� ������, ������� �� �����-�� ������� ��������� �������
                        .OrderBy(p => p.Name);                    // ����������� �� �����
            // ������� ������, ��������� � ��������
            foreach (var t in types)
            {
                var module = Activator.CreateInstance(t) as IModule;
                mm.AddModule(module);
            }
        }
        #endregion

        #region "Menu functions"
        private static void DoSettings()
        {
            var dlg = new Forms.SettingsDialog(cm);
            NppUtils.RegisterAsDialog(dlg.Handle);
            try
            {
                dlg.ShowDialog();
            }
            finally
            {
                NppUtils.UnregisterAsDialog(dlg.Handle);
            }
        }

        private static void DoAbout()
        {
            var dlg = new Forms.About();
            NppUtils.RegisterAsDialog(dlg.Handle);
            try
            {
                dlg.ShowDialog();
            }
            finally
            {
                NppUtils.UnregisterAsDialog(dlg.Handle);
            }
        }
        #endregion
    }
}