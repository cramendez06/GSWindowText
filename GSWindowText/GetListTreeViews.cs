using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;

namespace GSWindowText
{
    class GetListTreeViews
    {
        ///----------------------------------------------------------------------/
        ///
        /// System name：GSWindowText
        /// Program title：GetListTreeViews
        /// Overview：Gets the Listview and Treeview Items 
        ///
        ///      Author： M.Mendez    CREATE 2017/12/01          【P-10000】
        ///      
        ///     Copyright (C)
        ///----------------------------------------------------------------------/
        //=======================================================================
        //                        Win32 API Declarations
        //======================================================================= 
        #region Win32 API Declarations
        public const uint LVM_FIRST = 0x1000;
        public const uint LVM_GETITEMCOUNT = LVM_FIRST + 4;
        public const uint LVM_GETITEMW = LVM_FIRST + 75;

        public const uint PROCESS_VM_OPERATION = 0x0008;
        public const uint PROCESS_VM_READ = 0x0010;
        public const uint PROCESS_VM_WRITE = 0x0020;

        public const uint MEM_COMMIT = 0x1000;
        public const uint MEM_RELEASE = 0x8000;

        public const uint MEM_RESERVE = 0x2000;
        public const uint PAGE_READWRITE = 4;

        public static int LVIF_TEXT = 0x0001;

        public const int TV_FIRST = 0x1100;
        public const int TVM_GETCOUNT = TV_FIRST + 5;
        public const int TVM_GETNEXTITEM = TV_FIRST + 10;
        public const int TVM_GETITEMA = TV_FIRST + 12;
        public const int TVM_GETITEMW = TV_FIRST + 62;

        public const int TVGN_ROOT = 0x0000;
        public const int TVGN_NEXT = 0x0001;
        public const int TVGN_PREVIOUS = 0x0002;
        public const int TVGN_PARENT = 0x0003;
        public const int TVGN_CHILD = 0x0004;
        public const int TVGN_FIRSTVISIBLE = 0x0005;
        public const int TVGN_NEXTVISIBLE = 0x0006;
        public const int TVGN_PREVIOUSVISIBLE = 0x0007;
        public const int TVGN_DROPHILITE = 0x0008;
        public const int TVGN_CARET = 0x0009;
        public const int TVGN_LASTVISIBLE = 0x000A;

        public const int TVIF_TEXT = 0x0001;
        public const int TVIF_IMAGE = 0x0002;
        public const int TVIF_PARAM = 0x0004;
        public const int TVIF_STATE = 0x0008;
        public const int TVIF_HANDLE = 0x0010;
        public const int TVIF_SELECTEDIMAGE = 0x0020;
        public const int TVIF_CHILDREN = 0x0040;
        public const int TVIF_INTEGRAL = 0x0080;


        [DllImport("user32.DLL")]
        public static extern int SendMessage(IntPtr hWnd, uint Msg, int wParam, int lParam);

        [DllImport("user32.dll")]
        public static extern uint GetWindowThreadProcessId(IntPtr hWnd,
            out uint dwProcessId);

        [DllImport("kernel32.dll")]
        public static extern IntPtr OpenProcess(uint dwDesiredAccess,
            bool bInheritHandle, uint dwProcessId);

        [DllImport("kernel32.dll")]
        public static extern IntPtr VirtualAllocEx(IntPtr hProcess, IntPtr lpAddress,
            uint dwSize, uint flAllocationType, uint flProtect);

        [DllImport("kernel32.dll")]
        public static extern bool VirtualFreeEx(IntPtr hProcess, IntPtr lpAddress,
           uint dwSize, uint dwFreeType);

        [DllImport("kernel32.dll")]
        public static extern bool CloseHandle(IntPtr handle);

        [DllImport("kernel32.dll")]
        public static extern bool WriteProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress,
           IntPtr lpBuffer, int nSize, ref uint vNumberOfBytesRead);

        [DllImport("kernel32.dll")]
        public static extern bool ReadProcessMemory(IntPtr hProcess, IntPtr lpBaseAddress,
           IntPtr lpBuffer, int nSize, ref uint vNumberOfBytesRead);

        #region Helpers
        public struct LVITEM
        {
            public int mask;
            public int iItem;
            public int iSubItem;
            public int state;
            public int stateMask;
            public IntPtr pszText; // string 
            public int cchTextMax;
            public int iImage;
            public IntPtr lParam;
            public int iIndent;
            public int iGroupId;
            public int cColumns;
            public IntPtr puColumns;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct TVITEM
        {
            public int mask;
            public IntPtr hItem;
            public int state;
            public int stateMask;
            public IntPtr pszText;
            public int cchTextMax;
            public int iImage;
            public int iSelectedImage;
            public int cChildren;
            public IntPtr lParam;
            public IntPtr HTreeItem;
        }
        #endregion

        #endregion

        /// <summary>
        /// Gets ListView Item count
        /// </summary>
        /// <param name="p_AHandle">Handle to get the List count from</param>
        /// <returns>List item count in int</returns>
        public static int ListView_GetItemCount(IntPtr p_AHandle)
        {
            return SendMessage(p_AHandle, LVM_GETITEMCOUNT, 0, 0);
        }

        /// <summary>
        /// Get List view items
        /// </summary>
        /// <param name="p_hWnd">Handle to get the List from</param>
        /// <returns>List view items in string</returns>
        public static string GetListViewItems(IntPtr p_hWnd)
        {
            IntPtr w_vHandle = p_hWnd;
            StringBuilder retVal = new StringBuilder();

            if (w_vHandle == IntPtr.Zero) return string.Empty;
            int w_vItemCount = ListView_GetItemCount(w_vHandle);
            uint w_vProcessId;
            GetWindowThreadProcessId(w_vHandle, out w_vProcessId);

            IntPtr w_vProcess = OpenProcess(PROCESS_VM_OPERATION | PROCESS_VM_READ |
                PROCESS_VM_WRITE, false, w_vProcessId);
            IntPtr w_vPointer = VirtualAllocEx(w_vProcess, IntPtr.Zero, 4096,
                MEM_RESERVE | MEM_COMMIT, PAGE_READWRITE);
            try
            {
                for (int i = 0; i < w_vItemCount; i++)
                {
                    byte[] w_vBuffer = new byte[256];
                    LVITEM[] w_vItem = new LVITEM[1];
                    w_vItem[0].mask = LVIF_TEXT;
                    w_vItem[0].iItem = i;
                    w_vItem[0].iSubItem = 0;
                    w_vItem[0].cchTextMax = w_vBuffer.Length;
                    w_vItem[0].pszText = (IntPtr)((int)w_vPointer + Marshal.SizeOf(typeof(LVITEM)));
                    uint w_vNumberOfBytesRead = 0;

                    WriteProcessMemory(w_vProcess, w_vPointer,
                        Marshal.UnsafeAddrOfPinnedArrayElement(w_vItem, 0),
                        Marshal.SizeOf(typeof(LVITEM)), ref w_vNumberOfBytesRead);
                    SendMessage(w_vHandle, LVM_GETITEMW, i, w_vPointer.ToInt32());
                    ReadProcessMemory(w_vProcess,
                        (IntPtr)((int)w_vPointer + Marshal.SizeOf(typeof(LVITEM))),
                        Marshal.UnsafeAddrOfPinnedArrayElement(w_vBuffer, 0),
                        w_vBuffer.Length, ref w_vNumberOfBytesRead);

                    string w_vText = Marshal.PtrToStringUni(
                        Marshal.UnsafeAddrOfPinnedArrayElement(w_vBuffer, 0));
                    retVal.AppendLine(w_vText);
                }
            }
            finally
            {
                VirtualFreeEx(w_vProcess, w_vPointer, 0, MEM_RELEASE);
                CloseHandle(w_vProcess);
            }

            return retVal.ToString();
        }

        /// <summary>
        /// Get Treeview Item count
        /// </summary>
        /// <param name="p_hwnd">Handle to get the items count from</param>
        /// <returns>Item count in int</returns>
        public static uint TreeView_GetCount(IntPtr p_hwnd)
        {
            return (uint)SendMessage(p_hwnd, TVM_GETCOUNT, 0, 0);
        }

        /// <summary>
        /// Retrieve next item in treeview
        /// </summary>
        /// <param name="p_hwnd">Handle to get the next item from</param>
        /// <param name="p_hitem">Item number</param>
        /// <param name="p_code">Code number</param>
        /// <returns>Handle for the next item</returns>
        public static IntPtr TreeView_GetNextItem(IntPtr p_hwnd, IntPtr p_hitem, int p_code)
        {
            return (IntPtr)SendMessage(p_hwnd, TVM_GETNEXTITEM, p_code, (int)p_hitem);
        }

        /// <summary>
        /// Get the root node from the treeview
        /// </summary>
        /// <param name="p_hwnd">Handle to get the Root from</param>
        /// <returns>Handle for the Root node</returns>
        public static IntPtr TreeView_GetRoot(IntPtr p_hwnd)
        {
            return TreeView_GetNextItem(p_hwnd, IntPtr.Zero, TVGN_ROOT);
        }

        /// <summary>
        /// Get the child of the current item
        /// </summary>
        /// <param name="p_hwnd">Handle of the treeview</param>
        /// <param name="p_hitem">Handle of the parent node</param>
        /// <returns>Handle of child node</returns>
        public static IntPtr TreeView_GetChild(IntPtr p_hwnd, IntPtr p_hitem)
        {
            return TreeView_GetNextItem(p_hwnd, p_hitem, TVGN_CHILD);
        }

        /// <summary>
        /// Gets the sibling item
        /// </summary>
        /// <param name="p_hwnd">Handle of treeview</param>
        /// <param name="p_hitem">Current node</param>
        /// <returns>Handle of next child node</returns>
        public static IntPtr TreeView_GetNextSibling(IntPtr p_hwnd, IntPtr p_hitem)
        {
            return TreeView_GetNextItem(p_hwnd, p_hitem, TVGN_NEXT);
        }

        /// <summary>
        /// Gets the parent node of the child node
        /// </summary>
        /// <param name="p_hwnd">Handle of treeview</param>
        /// <param name="p_hitem"></param>
        /// <returns>Handle of parent node</returns>
        public static IntPtr TreeView_GetParent(IntPtr p_hwnd, IntPtr p_hitem)
        {
            return TreeView_GetNextItem(p_hwnd, p_hitem, TVGN_PARENT);
        }

        /// <summary>
        /// Get next treenode in treeview
        /// </summary>
        /// <param name="AHandle">Handle of treeview</param>
        /// <param name="ATreeItem">Handle of current treeitem</param>
        /// <returns>Handle of next treenode</returns>
        public static IntPtr TreeNodeGetNext(IntPtr p_AHandle, IntPtr p_ATreeItem)
        {
            if (p_AHandle == IntPtr.Zero || p_ATreeItem == IntPtr.Zero) return IntPtr.Zero;
            IntPtr result = TreeView_GetChild(p_AHandle, p_ATreeItem);
            if (result == IntPtr.Zero)
                result = TreeView_GetNextSibling(p_AHandle, p_ATreeItem);

            IntPtr vParentID = p_ATreeItem;
            while (result == IntPtr.Zero && vParentID != IntPtr.Zero)
            {
                vParentID = TreeView_GetParent(p_AHandle, vParentID);
                result = TreeView_GetNextSibling(p_AHandle, vParentID);
            }
            return result;
        }

        /// <summary>
        /// Get treeview text
        /// </summary>
        /// <param name="AHandle">Handle of treeview</param>
        /// <param name="AOutput">Output of treeview list string</param>
        /// <returns>Treeview items as string</returns>
        public static bool GetTreeViewText(IntPtr p_AHandle, ref List<string> p_AOutput)
        {
            if (p_AOutput == null) return false;
            uint vProcessId;
            GetWindowThreadProcessId(p_AHandle, out vProcessId);

            IntPtr vProcess = OpenProcess(PROCESS_VM_OPERATION | PROCESS_VM_READ |
                PROCESS_VM_WRITE, false, vProcessId);
            IntPtr vPointer = VirtualAllocEx(vProcess, IntPtr.Zero, 4096,
                MEM_RESERVE | MEM_COMMIT, PAGE_READWRITE);
            try
            {
                uint vItemCount = TreeView_GetCount(p_AHandle);
                IntPtr vTreeItem = TreeView_GetRoot(p_AHandle);
                Console.WriteLine(vItemCount);
                for (int i = 0; i < vItemCount; i++)
                {
                    byte[] vBuffer = new byte[256];
                    TVITEM[] vItem = new TVITEM[1];
                    vItem[0] = new TVITEM();
                    vItem[0].mask = TVIF_TEXT;
                    vItem[0].hItem = vTreeItem;
                    vItem[0].pszText = (IntPtr)((int)vPointer + Marshal.SizeOf(typeof(TVITEM)));
                    vItem[0].cchTextMax = vBuffer.Length;
                    uint vNumberOfBytesRead = 0;
                    WriteProcessMemory(vProcess, vPointer,
                        Marshal.UnsafeAddrOfPinnedArrayElement(vItem, 0),
                        Marshal.SizeOf(typeof(TVITEM)), ref vNumberOfBytesRead);
                    SendMessage(p_AHandle, TVM_GETITEMA, 0, (int)vPointer);
                    ReadProcessMemory(vProcess,
                        (IntPtr)((int)vPointer + Marshal.SizeOf(typeof(TVITEM))),
                        Marshal.UnsafeAddrOfPinnedArrayElement(vBuffer, 0),
                        vBuffer.Length, ref vNumberOfBytesRead);
                    p_AOutput.Add(Marshal.PtrToStringAnsi(
                        Marshal.UnsafeAddrOfPinnedArrayElement(vBuffer, 0)));

                    vTreeItem = TreeNodeGetNext(p_AHandle, vTreeItem);
                }
            }
            finally
            {
                VirtualFreeEx(vProcess, vPointer, 0, MEM_RELEASE);
                CloseHandle(vProcess);
            }
            return true;
        }

        /// <summary>
        /// Get all treeview items
        /// </summary>
        /// <param name="p_hWnd">Handle of treeview</param>
        /// <returns>Treeview List in string</returns>
        public static string GetTreeViewItems(IntPtr p_hWnd)
        {
            StringBuilder retVal = new StringBuilder();
            List<string> vOutput = new List<string>();
            GetTreeViewText(p_hWnd, ref vOutput);
            foreach (string vLine in vOutput)
                retVal.AppendLine(vLine);

            return retVal.ToString();
        }
    }
}
