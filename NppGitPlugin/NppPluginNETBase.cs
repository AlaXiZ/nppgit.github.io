﻿using System;

namespace NppGitPlugin
{
    public partial class PluginUtils
    {
        //private static Logger logger = LogManager.GetCurrentClassLogger();
        #region " Fields "
        public static NppData nppData;
        internal static FuncItems _funcItems = new FuncItems();
        internal static int index = 0;
        #endregion

        #region " Helper "
        public static int SetCommand(string commandName, Action functionPointer)
        {
            return SetCommand(commandName, functionPointer, new ShortcutKey(), false);
        }

        public static int SetCommand(string commandName, Action functionPointer, ShortcutKey shortcut)
        {
            return SetCommand(commandName, functionPointer, shortcut, false);
        }

        public static int SetCommand(string commandName, Action functionPointer, bool checkOnInit)
        {
            return SetCommand(commandName, functionPointer, new ShortcutKey(), checkOnInit);
        }

        public static int SetCommand(string commandName, Action functionPointer, ShortcutKey shortcut, bool checkOnInit)
        {
            //logger.Trace("Add command: " + commandName + " Shortcut Key: " + shortcut.ToString());
            FuncItem funcItem = new FuncItem();
            funcItem._cmdID = index++;
            funcItem._itemName = commandName;
            if (functionPointer != null)
                funcItem._pFunc = functionPointer;
            if (shortcut._key != 0)
                funcItem._pShKey = shortcut;
            funcItem._init2Check = checkOnInit;
            _funcItems.Add(funcItem);
            return (index - 1); // funcItem._cmdID;
        }
        #endregion
    }
}
