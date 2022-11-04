using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Security.Permissions;
using System.Threading;
using System.Windows.Forms;
using DirectShow;
using DirectShow.BaseClasses;

namespace Sonic
{
    #region Basic COM helper utilites classes

    [ComVisible(false)]
    public class COMHelper
    {
        #region BOOL

        public static BOOL TRUE => (BOOL)1;
        public static BOOL FALSE => (BOOL)0;

        #endregion

        #region FOURCC

        public static FOURCC MAKEFOURCC(char a1, char a2, char a3, char a4)
        {
            return new FOURCC(a1, a2, a3, a4);
        }

        public static FOURCC MAKEFOURCC(byte a1, byte a2, byte a3, byte a4)
        {
            return new FOURCC(a1, a2, a3, a4);
        }

        public static readonly FOURCC BI_RGB = (FOURCC)0;

        #endregion

        #region Helper Methods and constants

        public const int WM_NULL = 0x0000;
        public const int TIME_ONESHOT = 0x0000;
        public const int TIME_PERIODIC = 0x0001;
        public const int TIME_KILL_SYNCHRONOUS = 0x0100;
        public const object NULL = null;

#if DEBUG
        public static void _TRACE(string _message)
        {
            if (!string.IsNullOrEmpty(_message)) Trace.WriteLine(_message);
        }

        public static void TRACE(string _message)
        {
            if (!string.IsNullOrEmpty(_message))
            {
                _message += "\n";
                API.OutputDebugString(_message);
            }
        }

        public static void TRACE_ENTER()
        {
            var _method = new StackTrace(1, false).GetFrame(0).GetMethod();
            TRACE(string.Format("{0}::{1}", _method.ReflectedType.Name, _method.Name));
        }

        public static void ASSERT(object _object)
        {
            if (_object is BOOL) Debug.Assert((bool)_object);
            if (_object is HRESULT) Debug.Assert((bool)_object);
            else if (_object is bool) Debug.Assert((bool)_object);
            else Debug.Assert(_object != null);
        }
#else
        public static void _TRACE(string _message) { }
        public static void TRACE(string _message) { }
        public static void TRACE_ENTER() {}
        public static void ASSERT(object _object) { }
#endif
        public static bool SUCCEEDED(int hr)
        {
            return hr >= 0;
        }

        public static bool FAILED(int hr)
        {
            return hr < 0;
        }

        public static int HIWORD(int _value)
        {
            return (_value >> 16) & 0xffff;
        }

        public static int LOWORD(int _value)
        {
            return _value & 0xffff;
        }

        public const long MAX_LONG = 0x7FFFFFFFFFFFFFFF;

        public const long MILLISECONDS = 1000; // 10 ^ 3
        public const long NANOSECONDS = 1000000000; // 10 ^ 9
        public const long UNITS = NANOSECONDS / 100; // 10 ^ 7

        #region LCID

        public const int LANG_NEUTRAL = 0x00;
        public const int LANG_INVARIANT = 0x7f;

        public const int SUBLANG_NEUTRAL = 0x00;
        public const int SUBLANG_DEFAULT = 0x01;
        public const int SUBLANG_SYS_DEFAULT = 0x02;
        public const int SUBLANG_CUSTOM_DEFAULT = 0x03;
        public const int SUBLANG_CUSTOM_UNSPECIFIED = 0x04;
        public const int SUBLANG_UI_CUSTOM_DEFAULT = 0x05;

        public const int SORT_DEFAULT = 0x0;
        public const int SORT_INVARIANT_MATH = 0x1;

        public static int LANG_SYSTEM_DEFAULT => MAKELANGID(LANG_NEUTRAL, SUBLANG_SYS_DEFAULT);
        public static int LANG_USER_DEFAULT => MAKELANGID(LANG_NEUTRAL, SUBLANG_DEFAULT);

        public static int LOCALE_SYSTEM_DEFAULT => MAKELCID(LANG_SYSTEM_DEFAULT, SORT_DEFAULT);
        public static int LOCALE_USER_DEFAULT => MAKELCID(LANG_USER_DEFAULT, SORT_DEFAULT);

        public static int LOCALE_CUSTOM_DEFAULT =>
            MAKELCID(MAKELANGID(LANG_NEUTRAL, SUBLANG_CUSTOM_DEFAULT), SORT_DEFAULT);

        public static int LOCALE_CUSTOM_UNSPECIFIED =>
            MAKELCID(MAKELANGID(LANG_NEUTRAL, SUBLANG_CUSTOM_UNSPECIFIED), SORT_DEFAULT);

        public static int LOCALE_CUSTOM_UI_DEFAULT =>
            MAKELCID(MAKELANGID(LANG_NEUTRAL, SUBLANG_UI_CUSTOM_DEFAULT), SORT_DEFAULT);

        public static int LOCALE_NEUTRAL => MAKELCID(MAKELANGID(LANG_NEUTRAL, SUBLANG_NEUTRAL), SORT_DEFAULT);
        public static int LOCALE_INVARIANT => MAKELCID(MAKELANGID(LANG_INVARIANT, SUBLANG_NEUTRAL), SORT_DEFAULT);

        public static int MAKELANGID(int p, int s)
        {
            return ((ushort)s << 10) | (ushort)p;
        }

        public static int PRIMARYLANGID(int lgid)
        {
            return (ushort)lgid & 0x3ff;
        }

        public static int SUBLANGID(int lgid)
        {
            return (ushort)lgid >> 10;
        }

        public static int MAKELCID(int lgid, int srtid)
        {
            return ((ushort)srtid << 16) | (ushort)lgid;
        }

        public static int MAKESORTLCID(int lgid, int srtid, int ver)
        {
            return MAKELCID(lgid, srtid) | ((ushort)ver << 20);
        }

        public static int LANGIDFROMLCID(int lcid)
        {
            return (ushort)lcid;
        }

        public static int SORTIDFROMLCID(int lcid)
        {
            return (ushort)((lcid >> 16) & 0xf);
        }

        public static int SORTVERSIONFROMLCID(int lcid)
        {
            return (ushort)((lcid >> 20) & 0xf);
        }

        #endregion

        #region Align

        public static int ALIGN16(int SZ)
        {
            unchecked
            {
                return ((SZ + 15) >> 4) << 4;
            }
        } // round up to a multiple of 16

        public static int ALIGN32(int SZ)
        {
            unchecked
            {
                return ((SZ + 31) >> 5) << 5;
            }
        } // round up to a multiple of 32

        public static uint ALIGN16(uint SZ)
        {
            unchecked
            {
                return ((SZ + 15) >> 4) << 4;
            }
        } // round up to a multiple of 16

        public static uint ALIGN32(uint SZ)
        {
            unchecked
            {
                return ((SZ + 31) >> 5) << 5;
            }
        } // round up to a multiple of 32

        public static long ALIGN16(long SZ)
        {
            unchecked
            {
                return ((SZ + 15) >> 4) << 4;
            }
        } // round up to a multiple of 16

        public static long ALIGN32(long SZ)
        {
            unchecked
            {
                return ((SZ + 31) >> 5) << 5;
            }
        } // round up to a multiple of 32

        public static ulong ALIGN16(ulong SZ)
        {
            unchecked
            {
                return ((SZ + 15) >> 4) << 4;
            }
        } // round up to a multiple of 16

        public static ulong ALIGN32(ulong SZ)
        {
            unchecked
            {
                return ((SZ + 31) >> 5) << 5;
            }
        } // round up to a multiple of 32

        #endregion

        #endregion

        #region HRESULT

        public static HRESULT S_OK => (HRESULT)0;
        public static HRESULT S_FALSE => (HRESULT)1;
        public static HRESULT NOERROR => S_OK;
        public static HRESULT E_INVALIDARG => (HRESULT)0x80070057;

        public static HRESULT E_NOINTERFACE => (HRESULT)0x80004002;
        public static HRESULT E_NOTIMPL => (HRESULT)0x80004001;

        public static HRESULT E_UNEXPECTED => (HRESULT)0x8000FFFF;

        public static HRESULT E_FAIL => (HRESULT)0x80004005;

        public static HRESULT E_POINTER => (HRESULT)0x80004003;

        public static HRESULT E_OUTOFMEMORY => (HRESULT)0x8007000E;
        public static HRESULT E_FILE_NOT_FOUND => (HRESULT)0x80070002;

        public static HRESULT VFW_E_NOT_FOUND => (HRESULT)0x80040216;

        public static HRESULT VFW_E_CANNOT_CONNECT => (HRESULT)0x80040217;

        public static HRESULT VFW_E_NOT_CONNECTED => (HRESULT)0x80040209;
        public static HRESULT VFW_E_TYPE_NOT_ACCEPTED => (HRESULT)0x8004022A;
        public static HRESULT VFW_E_NO_ACCEPTABLE_TYPES => (HRESULT)0x80040207;

        public static HRESULT VFW_E_INVALID_DIRECTION => (HRESULT)0x80040208;

        public static HRESULT VFW_E_ALREADY_CONNECTED => (HRESULT)0x80040204;

        public static HRESULT VFW_E_NOT_STOPPED => (HRESULT)0x80040224;
        public static HRESULT VFW_E_NO_ALLOCATOR => (HRESULT)0x8004020A;

        public static HRESULT VFW_S_NO_MORE_ITEMS => (HRESULT)0x00040103;
        public static HRESULT VFW_E_INVALIDMEDIATYPE => (HRESULT)0x80040200;

        public static HRESULT VFW_E_RUNTIME_ERROR => (HRESULT)0x8004020B;

        public static HRESULT VFW_E_WRONG_STATE => (HRESULT)0x80040227;

        public static HRESULT VFW_E_ENUM_OUT_OF_SYNC => (HRESULT)0x80040203;
        public static HRESULT VFW_E_NO_CLOCK => (HRESULT)0x80040213;

        public static HRESULT VFW_E_NOT_IN_GRAPH => (HRESULT)0x8004025F;

        public static HRESULT VFW_E_START_TIME_AFTER_END => (HRESULT)0x80040228;

        public static HRESULT VFW_S_STATE_INTERMEDIATE => (HRESULT)0x00040237;

        public static HRESULT VFW_E_SAMPLE_REJECTED => (HRESULT)0x8004022B;

        public static HRESULT VFW_E_STATE_CHANGED => (HRESULT)0x80040223;
        public static HRESULT VFW_E_SAMPLE_TIME_NOT_SET => (HRESULT)0x80040249;

        public static HRESULT VFW_E_TIMEOUT => (HRESULT)0x8004022E;

        public static HRESULT VFW_S_NO_STOP_TIME => (HRESULT)0x00040270;

        #endregion

        #region Enum

        [ComVisible(false)]
        [Flags]
        public enum CLSCTX : uint
        {
            CLSCTX_INPROC_SERVER = 0x1,
            CLSCTX_INPROC_HANDLER = 0x2,
            CLSCTX_LOCAL_SERVER = 0x4,
            CLSCTX_INPROC_SERVER16 = 0x8,
            CLSCTX_REMOTE_SERVER = 0x10,
            CLSCTX_INPROC_HANDLER16 = 0x20,
            CLSCTX_RESERVED1 = 0x40,
            CLSCTX_RESERVED2 = 0x80,
            CLSCTX_RESERVED3 = 0x100,
            CLSCTX_RESERVED4 = 0x200,
            CLSCTX_NO_CODE_DOWNLOAD = 0x400,
            CLSCTX_RESERVED5 = 0x800,
            CLSCTX_NO_CUSTOM_MARSHAL = 0x1000,
            CLSCTX_ENABLE_CODE_DOWNLOAD = 0x2000,
            CLSCTX_NO_FAILURE_LOG = 0x4000,
            CLSCTX_DISABLE_AAA = 0x8000,
            CLSCTX_ENABLE_AAA = 0x10000,
            CLSCTX_FROM_DEFAULT_CONTEXT = 0x20000,
            CLSCTX_ACTIVATE_32_BIT_SERVER = 0x40000,
            CLSCTX_ACTIVATE_64_BIT_SERVER = 0x80000,
            CLSCTX_INPROC = CLSCTX_INPROC_SERVER | CLSCTX_INPROC_HANDLER,
            CLSCTX_SERVER = CLSCTX_INPROC_SERVER | CLSCTX_LOCAL_SERVER | CLSCTX_REMOTE_SERVER,
            CLSCTX_ALL = CLSCTX_SERVER | CLSCTX_INPROC_HANDLER
        }

        #endregion

        #region API

        [ComVisible(false)]
        public static class API
        {
            [DllImport("kernel32.dll", EntryPoint = "OutputDebugStringW", CharSet = CharSet.Unicode)]
            public static extern void OutputDebugString(string _text);

            [DllImport("kernel32.dll", EntryPoint = "RtlMoveMemory")]
            public static extern void CopyMemory(IntPtr Destination, IntPtr Source,
                [MarshalAs(UnmanagedType.U4)] int Length);

            [DllImport("ole32.dll")]
            public static extern int CreateBindCtx(int reserved, out IBindCtx ppbc);

            [DllImport("ole32.dll", CharSet = CharSet.Unicode)]
            public static extern int MkParseDisplayName(IBindCtx pbc, string szUserName, ref int pchEaten,
                out IMoniker ppmk);

            [DllImport("olepro32.dll")]
            public static extern int OleCreatePropertyFrame(
                IntPtr hwndOwner,
                int x,
                int y,
                [MarshalAs(UnmanagedType.LPWStr)] string lpszCaption,
                int cObjects,
                [MarshalAs(UnmanagedType.Interface, ArraySubType = UnmanagedType.IUnknown)]
                ref object ppUnk,
                int cPages,
                IntPtr lpPageClsID,
                int lcid,
                int dwReserved,
                IntPtr lpvReserved
            );

            [DllImport("ole32.dll")]
            public static extern int CoCreateInstance(
                [In] [MarshalAs(UnmanagedType.LPStruct)]
                Guid rclsid,
                IntPtr pUnkOuter,
                CLSCTX dwClsContext,
                [In] [MarshalAs(UnmanagedType.LPStruct)]
                Guid riid,
                out IntPtr rReturnedComObject
            );
        }

        #endregion
    }

    [ComVisible(false)]
    public sealed class BOOL : COMHelper, IComparable, ICloneable
    {
        #region Variables

        private bool m_Result;

        #endregion

        #region ICloneable Members

        public object Clone()
        {
            return new BOOL(m_Result);
        }

        #endregion

        #region IComparable Members

        public int CompareTo(object obj)
        {
            if (obj is BOOL) return m_Result.CompareTo(((BOOL)obj).m_Result);
            return m_Result.CompareTo(obj);
        }

        #endregion

        #region Methods

        public void Assert()
        {
            ASSERT(m_Result);
        }

        #endregion

        #region Constructor

        public BOOL()
        {
        }

        public BOOL(int _value)
        {
            m_Result = _value == 0 ? false : true;
        }

        public BOOL(bool _value)
        {
            m_Result = _value;
        }

        public BOOL(BOOL _value)
            : this(_value.m_Result)
        {
        }

        #endregion

        #region Overridden Methods

        public override bool Equals(object obj)
        {
            if (obj is BOOL) return m_Result.Equals(((BOOL)obj).m_Result);
            return m_Result.Equals(obj);
        }

        public override int GetHashCode()
        {
            return m_Result.GetHashCode();
        }

        public override string ToString()
        {
            return m_Result.ToString();
        }

        #endregion

        #region Static Methods

        public new static BOOL TRUE => COMHelper.TRUE;

        public new static BOOL FALSE => COMHelper.FALSE;

        #endregion

        #region Operators

        public static bool operator ==(BOOL _src, BOOL _dest)
        {
            if (ReferenceEquals(_src, _dest)) return true;
            if (_src == _dest as object) return true;

            if (_src as object != null && _dest as object != null) return _src.m_Result == _dest.m_Result;
            return false;
        }

        public static bool operator !=(BOOL _src, BOOL _dest)
        {
            return !(_src == _dest);
        }

        public static bool operator ==(BOOL _src, HRESULT _dest)
        {
            return _src == (BOOL)_dest;
        }

        public static bool operator !=(BOOL _src, HRESULT _dest)
        {
            return !(_src == _dest);
        }

        public static implicit operator bool(BOOL _bool)
        {
            if (_bool as object == null) return false;
            return _bool.m_Result;
        }

        public static implicit operator int(BOOL _bool)
        {
            if (_bool as object == null) return 0;
            return _bool.m_Result ? 1 : 0;
        }

        public static implicit operator HRESULT(BOOL _bool)
        {
            if (_bool as object == null) return S_FALSE;
            return _bool.m_Result ? S_OK : S_FALSE;
        }

        public static explicit operator BOOL(HRESULT hr)
        {
            if (hr as object == null) return FALSE;
            return new BOOL(hr == S_OK);
        }

        public static explicit operator BOOL(int _value)
        {
            return new BOOL(_value != 0);
        }

        public static explicit operator BOOL(bool _bool)
        {
            return new BOOL(_bool);
        }

        #endregion
    }

    [ComVisible(false)]
    public class HRESULT : COMHelper, IComparable, ICloneable
    {
        #region Variables

        private int m_Result;

        #endregion

        #region ICloneable Members

        public object Clone()
        {
            return new HRESULT(m_Result);
        }

        #endregion

        #region IComparable Members

        public int CompareTo(object obj)
        {
            if (obj is HRESULT) return m_Result.CompareTo(((HRESULT)obj).m_Result);
            return m_Result.CompareTo(obj);
        }

        #endregion

        #region Virtual Methods

        protected virtual string GetErrorText()
        {
            var _text = "";
            uint _length = 160;
            var _ptr = Marshal.AllocCoTaskMem((int)_length);
            try
            {
                if (AMGetErrorText(m_Result, _ptr, _length) > 0) _text = Marshal.PtrToStringAuto(_ptr);
            }
            catch
            {
            }

            Marshal.FreeCoTaskMem(_ptr);
            return _text;
        }

        #endregion

        #region Imports

        [DllImport("quartz.dll", EntryPoint = "AMGetErrorText", CharSet = CharSet.Auto)]
        private static extern uint AMGetErrorText(int hr, IntPtr sText, [In] uint _length);

        #endregion

        #region Constructor

        public HRESULT()
        {
        }

        public HRESULT(int hr)
        {
            m_Result = hr;
        }

        public HRESULT(HRESULT hr)
            : this(hr.m_Result)
        {
        }

        #endregion

        #region Properties

        public bool Failed => FAILED(m_Result);

        public bool Succeeded => SUCCEEDED(m_Result);

        public string Text => GetErrorText();

        #endregion

        #region Overridden Methods

        public override bool Equals(object obj)
        {
            if (obj is HRESULT) return m_Result.Equals(((HRESULT)obj).m_Result);
            return m_Result.Equals(obj);
        }

        public override int GetHashCode()
        {
            return m_Result.GetHashCode();
        }

        public override string ToString()
        {
            var _text = GetErrorText();
            if (_text != "") _text = " ( " + _text + " )";
            _text = "0x" + m_Result.ToString("x8") + _text;
            return _text;
        }

        #endregion

        #region Methods

        public void Assert()
        {
            ASSERT(SUCCEEDED(m_Result));
        }

        public void Throw()
        {
            if (FAILED(m_Result)) Marshal.ThrowExceptionForHR(m_Result);
        }

        public void TraceWrite()
        {
            if (Failed)
            {
                var _frame = new StackTrace(1, true).GetFrame(0);
                var _method = _frame.GetMethod();
                var _file = _frame.GetFileName();
                _file = !string.IsNullOrEmpty(_file) ? Path.GetFileName(_file) : "";
                TRACE(string.Format("-- ASSERT -- Method {0}::{1}, HRESULT : {2},File : {3} Line : {4} ",
                    _method.ReflectedType.Name,
                    _method.Name,
                    ToString(),
                    _file,
                    _frame.GetFileLineNumber()
                ));
            }
        }

        #endregion

        #region Operators

        public static bool operator ==(HRESULT _src, HRESULT _dest)
        {
            if (ReferenceEquals(_src, _dest)) return true;
            if (_src == _dest as object) return true;

            if (_src as object != null && _dest as object != null) return _src.m_Result == _dest.m_Result;
            return false;
        }

        public static bool operator !=(HRESULT _src, HRESULT _dest)
        {
            return !(_src == _dest);
        }

        public static implicit operator bool(HRESULT hr)
        {
            if (hr as object == null) return true;
            return SUCCEEDED(hr.m_Result);
        }

        public static implicit operator int(HRESULT hr)
        {
            if (hr as object == null) return 0;
            return hr.m_Result;
        }

        public static explicit operator HRESULT(int hr)
        {
            return new HRESULT(hr);
        }

        public static explicit operator HRESULT(uint hr)
        {
            unchecked
            {
                return new HRESULT((int)hr);
            }
        }

        #endregion
    }

    [ComVisible(false)]
    public sealed class FOURCC : COMHelper, IComparable, ICloneable
    {
        #region Variables

        private uint m_uiFourcc;

        #endregion

        #region ICloneable Members

        public object Clone()
        {
            return new FOURCC(m_uiFourcc);
        }

        #endregion

        #region IComparable Members

        public int CompareTo(object obj)
        {
            if (obj is FOURCC) return m_uiFourcc.CompareTo(((FOURCC)obj).m_uiFourcc);
            return m_uiFourcc.CompareTo(obj);
        }

        #endregion

        #region Constants

        private const ushort m_usB = 0x0000;
        private const ushort m_usC = 0x0010;
        private static readonly byte[] m_abtD = { 0x80, 0x00, 0x00, 0xAA, 0x00, 0x38, 0x9B, 0x71 };

        #endregion

        #region Constructors

        public FOURCC(int _value)
        {
            unchecked
            {
                m_uiFourcc = (uint)_value;
            }
        }

        public FOURCC(uint _value)
        {
            m_uiFourcc = _value;
        }

        public FOURCC(Guid _guid)
        {
            var _data = _guid.ToByteArray();
            byte[] _result = { _data[0], _data[1], _data[2], _data[3] };
            m_uiFourcc = BitConverter.ToUInt32(_result, 0);
        }

        public FOURCC(string _value)
        {
            var _data = new byte[4];
            for (var i = 0; i < _data.Length; i++)
                if (i < _value.Length)
                    _data[i] = (byte)_value[i];
                else
                    _data[i] = 0;
            m_uiFourcc = BitConverter.ToUInt32(_data, 0);
        }

        public FOURCC(byte[] _value)
        {
            var _data = new byte[4];
            for (var i = 0; i < _data.Length; i++)
                if (i < _value.Length)
                    _data[i] = _value[i];
                else
                    _data[i] = 0;
            m_uiFourcc = BitConverter.ToUInt32(_data, 0);
        }

        public FOURCC(char[] _value)
        {
            var _data = new byte[4];
            for (var i = 0; i < _data.Length; i++)
                if (i < _value.Length)
                    _data[i] = (byte)_value[i];
                else
                    _data[i] = 0;
            m_uiFourcc = BitConverter.ToUInt32(_data, 0);
        }

        public FOURCC(char a1, char a2, char a3, char a4)
        {
            byte[] _data = { (byte)a1, (byte)a2, (byte)a3, (byte)a4 };
            m_uiFourcc = BitConverter.ToUInt32(_data, 0);
        }

        public FOURCC(byte a1, byte a2, byte a3, byte a4)
        {
            byte[] _data = { a1, a2, a3, a4 };
            m_uiFourcc = BitConverter.ToUInt32(_data, 0);
        }

        #endregion

        #region Overridden Methods

        public override bool Equals(object obj)
        {
            if (obj is FOURCC) return m_uiFourcc.Equals(((FOURCC)obj).m_uiFourcc);
            return m_uiFourcc.Equals(obj);
        }

        public override int GetHashCode()
        {
            return m_uiFourcc.GetHashCode();
        }

        public override string ToString()
        {
            var _data = BitConverter.GetBytes(m_uiFourcc);
            var _result = " ( ";
            for (var i = 0; i < _data.Length; i++)
                if (_data[i] <= 30 || _data[i] > 128)
                {
                    _result = "";
                    break;
                }
                else
                {
                    _result += (char)_data[i];
                }

            if (_result != "") _result += " )";
            return m_uiFourcc.ToString("x4") + _result;
        }

        #endregion

        #region Operators

        public static bool operator ==(FOURCC _src, FOURCC _dest)
        {
            if (ReferenceEquals(_src, _dest)) return true;
            if (_src == _dest as object) return true;

            if (_src as object != null && _dest as object != null) return _src.m_uiFourcc == _dest.m_uiFourcc;
            return false;
        }

        public static bool operator !=(FOURCC _src, FOURCC _dest)
        {
            return !(_src == _dest);
        }

        public static explicit operator FOURCC(int _value)
        {
            return new FOURCC(_value);
        }

        public static explicit operator FOURCC(uint _value)
        {
            return new FOURCC(_value);
        }

        public static explicit operator FOURCC(Guid _value)
        {
            return new FOURCC(_value);
        }

        public static explicit operator FOURCC(string _value)
        {
            if (_value == null) return new FOURCC(0);
            return new FOURCC(_value);
        }

        public static explicit operator FOURCC(Array _value)
        {
            if (_value == null) return new FOURCC(0);
            var _data = new byte[4];
            for (var i = 0; i < _data.Length; i++)
                if (i < _value.Length)
                    _data[i] = Convert.ToByte(_value.GetValue(i));
                else
                    _data[i] = 0;
            return new FOURCC(_data);
        }

        public static implicit operator int(FOURCC _fcc)
        {
            if (_fcc as object == null) return 0;
            unchecked
            {
                return (int)_fcc.m_uiFourcc;
            }
        }

        public static implicit operator uint(FOURCC _fcc)
        {
            if (_fcc as object == null) return 0;
            return _fcc.m_uiFourcc;
        }

        public static implicit operator byte[](FOURCC _fcc)
        {
            if (_fcc as object == null) return null;
            var _data = BitConverter.GetBytes(_fcc.m_uiFourcc);
            var _result = new byte[_data.Length];
            for (var i = 0; i < _data.Length; i++) _result[3 - i] = _data[i];
            return _result;
        }

        public static implicit operator char[](FOURCC _fcc)
        {
            if (_fcc as object == null) return null;
            var _data = BitConverter.GetBytes(_fcc.m_uiFourcc);
            var _result = new char[_data.Length];
            for (var i = 0; i < _data.Length; i++) _result[3 - i] = (char)_data[i];
            return _result;
        }

        public static implicit operator string(FOURCC _fcc)
        {
            if (_fcc as object == null) return "";
            var _data = BitConverter.GetBytes(_fcc.m_uiFourcc);
            var _result = "";
            for (var i = 0; i < _data.Length; i++) _result += (char)_data[i];
            return _result;
        }

        public static implicit operator Guid(FOURCC _fcc)
        {
            if (_fcc as object == null) return Guid.Empty;
            return new Guid(_fcc.m_uiFourcc, m_usB, m_usC, m_abtD[0], m_abtD[1], m_abtD[2], m_abtD[3], m_abtD[4],
                m_abtD[5], m_abtD[6], m_abtD[7]);
        }

        #endregion
    }

    [ComVisible(false)]
    [SecurityPermission(SecurityAction.InheritanceDemand, UnmanagedCode = true)]
    [SecurityPermission(SecurityAction.Demand, UnmanagedCode = true)]
    public class VTableInterface : COMHelper, IDisposable
    {
        #region IDisposable Members

        public void Dispose()
        {
            if (m_pUnknown != IntPtr.Zero)
            {
                if (m_bShouldRelease) _Release();
                m_pUnknown = IntPtr.Zero;
            }
        }

        #endregion

        #region Helper Methods

        protected T GetProcDelegate<T>(int nIndex) where T : class
        {
            var pVtable = Marshal.ReadIntPtr(m_pUnknown);
            var pFunc = Marshal.ReadIntPtr(pVtable, nIndex * IntPtr.Size);
            return Marshal.GetDelegateForFunctionPointer(pFunc, typeof(T)) as T;
        }

        #endregion

        #region Delegates

        private delegate int QueryInterfaceProc(
            IntPtr pUnk,
            ref Guid riid,
            out IntPtr ppvObject
        );

        private delegate uint AddRefProc(IntPtr pUnk);

        #endregion

        #region Variables

        protected IntPtr m_pUnknown = IntPtr.Zero;
        protected bool m_bShouldRelease;

        #endregion

        #region Properties

        public IntPtr UnknownPtr => m_pUnknown;

        public bool IsValid => m_pUnknown != IntPtr.Zero;

        #endregion

        #region Constructor

        protected VTableInterface()
            : this(IntPtr.Zero, false)
        {
        }

        protected VTableInterface(IntPtr pUnknown, Type _type)
            : this(pUnknown, _type.GUID)
        {
        }

        protected VTableInterface(IntPtr pUnknown, Guid _guid)
            : this(IntPtr.Zero, false)
        {
            var hr = (HRESULT)Marshal.QueryInterface(pUnknown, ref _guid, out m_pUnknown);
            if (hr.Succeeded) Marshal.Release(pUnknown);
        }

        protected VTableInterface(IntPtr pUnknown)
            : this(pUnknown, false)
        {
        }

        protected VTableInterface(IntPtr pUnknown, bool bAddRef)
        {
            if (pUnknown != IntPtr.Zero)
            {
                m_pUnknown = pUnknown;
                if (bAddRef)
                {
                    m_bShouldRelease = true;
                    _AddRef();
                }
            }
        }

        protected VTableInterface(object _object, Guid _guid)
            : this(IntPtr.Zero, false)
        {
            if (_object != null)
            {
                var pUnknown = Marshal.GetIUnknownForObject(_object);
                var hr = (HRESULT)Marshal.QueryInterface(pUnknown, ref _guid, out m_pUnknown);
                if (hr.Succeeded) Marshal.Release(pUnknown);
            }
        }

        protected VTableInterface(object _object, Type _type)
            : this(_object, _type.GUID)
        {
        }

        ~VTableInterface()
        {
            Dispose();
        }

        #endregion

        #region IUnknown

        public HRESULT _QueryInterface(ref Guid riid, out IntPtr ppvObject)
        {
            ppvObject = IntPtr.Zero;

            if (m_pUnknown == IntPtr.Zero) return E_NOINTERFACE;

            var _Proc = GetProcDelegate<QueryInterfaceProc>(0);

            if (_Proc == null) return E_UNEXPECTED;

            return (HRESULT)_Proc(
                m_pUnknown,
                ref riid,
                out ppvObject
            );
        }

        public uint _AddRef()
        {
            if (m_pUnknown == IntPtr.Zero) return 0;

            var _Proc = GetProcDelegate<AddRefProc>(1);

            if (_Proc == null) return 0;
            return _Proc(m_pUnknown);
        }

        public uint _Release()
        {
            if (m_pUnknown == IntPtr.Zero) return 0;

            var _Proc = GetProcDelegate<AddRefProc>(2);
            if (_Proc == null) return 0;
            return _Proc(m_pUnknown);
        }

        public T _GetObject<T>() where T : class
        {
            var _guid = typeof(T).GUID;
            IntPtr _ptr;
            if (SUCCEEDED(_QueryInterface(ref _guid, out _ptr)))
            {
                _Release();
                return (T)Marshal.GetObjectForIUnknown(_ptr);
            }

            return null;
        }

        #endregion

        #region Methods

        public HRESULT IsInterface(Guid _guid)
        {
            IntPtr pvObject;
            var hr = _QueryInterface(ref _guid, out pvObject);
            if (hr.Succeeded) _Release();
            return hr;
        }

        public HRESULT IsInterface(Type _type)
        {
            return IsInterface(_type.GUID);
        }

        public T GetInterfaceImpl<T>(Guid _guid) where T : VTableInterface, new()
        {
            IntPtr pUnknown;
            if (SUCCEEDED(_QueryInterface(ref _guid, out pUnknown)))
            {
                var pT = new T();
                VTableInterface _interface = pT;
                _interface.m_pUnknown = pUnknown;
                return pT;
            }

            return null;
        }

        public T GetInterfaceImpl<T>(Type _type) where T : VTableInterface, new()
        {
            return GetInterfaceImpl<T>(_type.GUID);
        }

        public T GetInterfaceImpl<T>() where T : VTableInterface, new()
        {
            try
            {
                return GetInterfaceImpl<T>(typeof(T));
            }
            catch
            {
            }

            return null;
        }

        #endregion
    }

    [ComVisible(false)]
    [SecurityPermission(SecurityAction.InheritanceDemand, UnmanagedCode = true)]
    [SecurityPermission(SecurityAction.Demand, UnmanagedCode = true)]
    public class IUnknownImpl : VTableInterface
    {
        #region Constructor

        public IUnknownImpl(IntPtr pUnknown)
            : base(pUnknown, false)
        {
        }

        #endregion

        #region Methods

        public new T GetProcDelegate<T>(int nIndex) where T : class
        {
            return base.GetProcDelegate<T>(nIndex);
        }

        #endregion
    }

    #endregion

    #region Base COM object

    [ComVisible(false)]
    [SecurityPermission(SecurityAction.InheritanceDemand, UnmanagedCode = true)]
    [SecurityPermission(SecurityAction.Demand, UnmanagedCode = true)]
    public class DSObject<T> : COMHelper, IDisposable where T : class
    {
        #region IDisposable Members

        public virtual void Dispose()
        {
            if (m_bReleaseOnDestroy && m_pUnknown != null)
            {
                if (Marshal.IsComObject(m_pUnknown)) Marshal.ReleaseComObject(m_pUnknown);
                m_pUnknown = null;
            }
        }

        #endregion

        #region Variables

        protected T m_pUnknown;
        protected bool m_bReleaseOnDestroy;

        #endregion

        #region Properties

        public T Value
        {
            get => m_pUnknown;
            set => m_pUnknown = value;
        }

        public bool IsReleased => m_pUnknown == null;

        public bool IsValid => !IsReleased;

        public bool ReleaseOnDestroy
        {
            get => m_bReleaseOnDestroy;
            set => m_bReleaseOnDestroy = value;
        }

        public Guid GUID => typeof(T).GUID;

        public object this[Type _type] => QueryInterface(_type);

        #endregion

        #region Constructor

        public DSObject()
        {
            var _guid = GetType().GUID;
            if (_guid != Guid.Empty)
            {
                IntPtr _ptr;
                var hr = (HRESULT)API.CoCreateInstance(
                    _guid,
                    IntPtr.Zero,
                    CLSCTX.CLSCTX_INPROC_SERVER,
                    typeof(T).GUID,
                    out _ptr);
                if (hr.Succeeded)
                {
                    m_pUnknown = Marshal.GetObjectForIUnknown(_ptr) as T;
                    m_bReleaseOnDestroy = true;
                }
            }
        }

        public DSObject(Guid _guid)
        {
            if (_guid != Guid.Empty)
            {
                IntPtr _ptr;
                var hr = (HRESULT)API.CoCreateInstance(
                    _guid,
                    IntPtr.Zero,
                    CLSCTX.CLSCTX_INPROC_SERVER,
                    typeof(T).GUID,
                    out _ptr);
                if (hr.Succeeded)
                {
                    m_pUnknown = Marshal.GetObjectForIUnknown(_ptr) as T;
                    m_bReleaseOnDestroy = true;
                }
            }
        }

        protected DSObject(IntPtr _ptr)
        {
            if (_ptr != IntPtr.Zero)
            {
                m_pUnknown = (T)Marshal.GetObjectForIUnknown(_ptr);
                m_bReleaseOnDestroy = true;
            }
        }

        protected DSObject(T _unknown)
        {
            m_pUnknown = _unknown;
            m_bReleaseOnDestroy = true;
        }

        protected DSObject(T _unknown, bool bReleaseOnDestroy)
            : this(_unknown)
        {
            m_bReleaseOnDestroy = bReleaseOnDestroy;
        }

        ~DSObject()
        {
            Dispose();
        }

        #endregion

        #region Overriden Methods

        public override bool Equals(object obj)
        {
            if (obj == m_pUnknown) return true;
            if (obj is DSObject<T>) return (obj as DSObject<T>).m_pUnknown == m_pUnknown;
            return base.Equals(obj);
        }

        public override int GetHashCode()
        {
            if (m_pUnknown != null) return m_pUnknown.GetHashCode();
            return base.GetHashCode();
        }

        #endregion

        #region Public Methods

        public object QueryInterface(Guid _guid)
        {
            object _object = null;
            if (m_pUnknown != null)
            {
                var _this = Marshal.GetIUnknownForObject(m_pUnknown);
                var _interface = IntPtr.Zero;
                try
                {
                    var hr = Marshal.QueryInterface(_this, ref _guid, out _interface);

                    if (SUCCEEDED(hr) && _interface != IntPtr.Zero) _object = Marshal.GetObjectForIUnknown(_interface);
                }
                finally
                {
                    Marshal.Release(_this);
                    if (_interface != IntPtr.Zero) Marshal.Release(_interface);
                }
            }

            return _object;
        }

        public object QueryInterface(Type _type)
        {
            return QueryInterface(_type.GUID);
        }

        public bool IsSupported(Guid _guid)
        {
            if (m_pUnknown != null)
            {
                var _this = Marshal.GetIUnknownForObject(m_pUnknown);
                var _interface = IntPtr.Zero;
                try
                {
                    return SUCCEEDED(Marshal.QueryInterface(_this, ref _guid, out _interface));
                }
                finally
                {
                    Marshal.Release(_this);
                    if (_interface != IntPtr.Zero) Marshal.Release(_interface);
                }
            }

            return false;
        }

        public bool IsSupported(Type _type)
        {
            var _guid = _type.GUID;
            if (_guid != Guid.Empty) return IsSupported(_guid);
            return false;
        }

        #endregion

        #region Operators

        public static bool operator ==(DSObject<T> _src, DSObject<T> _dest)
        {
            if (ReferenceEquals(_src, _dest)) return true;
            if (_src == _dest as object) return true;

            if ((_src as object == null || _src.m_pUnknown == null)
                && (_dest as object == null || _dest.m_pUnknown == null))
                return true;
            if (_src as object != null && _dest as object != null) return _src.m_pUnknown == _dest.m_pUnknown;
            return false;
        }

        public static bool operator !=(DSObject<T> _src, DSObject<T> _dest)
        {
            return !(_src == _dest);
        }

        public static implicit operator T(DSObject<T> _object)
        {
            if ((object)_object == null) return null;
            return _object.m_pUnknown;
        }

        public static implicit operator IntPtr(DSObject<T> _object)
        {
            if (_object as object == null || _object.m_pUnknown == null) return IntPtr.Zero;
            return Marshal.GetIUnknownForObject(_object.m_pUnknown);
        }

        public static implicit operator bool(DSObject<T> _object)
        {
            return (object)_object != null && _object.m_pUnknown != null;
        }

        public static explicit operator DSObject<T>(IntPtr _ptr)
        {
            return new DSObject<T>(_ptr);
        }

        public static explicit operator DSObject<T>(T _object)
        {
            return new DSObject<T>(_object);
        }

        #endregion
    }

    #endregion

    #region Base Moniker Enumerator object

    [ComVisible(false)]
    public class MonikerInfo : DSObject<IMoniker>
    {
        #region Public Methods

        public object Bind(Type _type)
        {
            object _object = null;
            if (m_pUnknown != null)
            {
                var _guid = _type.GUID;
                if (_guid != Guid.Empty)
                    try
                    {
                        m_pUnknown.BindToObject(null, null, ref _guid, out _object);
                    }
                    catch
                    {
                        _object = null;
                    }
            }

            return _object;
        }

        #endregion

        #region Protected Methods

        protected string GetPropBagValue(string sPropName)
        {
            if (m_pUnknown == null) return null;
            IPropertyBag _property = null;
            try
            {
                object _object = null;
                var _guid = typeof(IPropertyBag).GUID;
                m_pUnknown.BindToStorage(null, null, ref _guid, out _object);
                _property = (IPropertyBag)_object;
                object _value;
                var hr = _property.Read(sPropName, out _value, null);
                ASSERT(hr == S_OK);
                if (SUCCEEDED(hr)) return _value as string;
            }
            catch
            {
            }
            finally
            {
                if (_property != null)
                {
                    Marshal.ReleaseComObject(_property);
                    _property = null;
                }
            }

            return null;
        }

        #endregion

        #region Fields

        public Guid ClassID
        {
            get
            {
                var _guid = Guid.Empty;
                if (m_pUnknown != null) m_pUnknown.GetClassID(out _guid);
                return _guid;
            }
        }

        public string Name => GetPropBagValue("FriendlyName");

        public string DevicePath
        {
            get
            {
                var _out = "";
                if (m_pUnknown != null) m_pUnknown.GetDisplayName(null, null, out _out);
                return _out;
            }
        }

        public string this[string _value] => GetPropBagValue(_value);

        #endregion

        #region Constructor

        public MonikerInfo()
            : base(IntPtr.Zero)
        {
        }

        public MonikerInfo(IMoniker _moniker)
            : base(_moniker)
        {
        }

        public MonikerInfo(string _moniker)
        {
            IBindCtx _context = null;
            try
            {
                if (SUCCEEDED(API.CreateBindCtx(0, out _context)))
                {
                    var n = 0;
                    ASSERT(SUCCEEDED(API.MkParseDisplayName(_context, _moniker, ref n, out m_pUnknown)));
                }
            }
            finally
            {
                if (_context != null) Marshal.ReleaseComObject(_context);
            }
        }

        #endregion
    }

    [ComVisible(false)]
    public class DSEnumerator<T> : DSObject<ICreateDevEnum>, IEnumerable<T> where T : MonikerInfo, new()
    {
        #region Constructor

        public DSEnumerator(Guid _category)
            : base(new Guid("62BE5D10-60EB-11d0-BD3B-00A0C911CE86"))
        {
            var hr = m_pUnknown.CreateClassEnumerator(ref _category, out m_pEnumMoniker, 0);
            ASSERT(hr == S_OK);
            if (SUCCEEDED(hr))
                if (hr != S_FALSE)
                {
                    var aMonikers = new IMoniker[1];
                    while (m_pEnumMoniker.Next(1, aMonikers, IntPtr.Zero) == 0)
                    {
                        var _moniker = new T();
                        _moniker.Value = aMonikers[0];
                        m_Objects.Add(_moniker);
                    }
                }
        }

        #endregion

        #region IEnumerable<T> Members

        public IEnumerator<T> GetEnumerator()
        {
            return m_Objects.GetEnumerator();
        }

        #endregion

        #region IEnumerable Members

        IEnumerator IEnumerable.GetEnumerator()
        {
            return m_Objects.GetEnumerator();
        }

        #endregion

        #region IDisposable Members

        public override void Dispose()
        {
            if (m_pEnumMoniker != null)
            {
                Marshal.ReleaseComObject(m_pEnumMoniker);
                m_pEnumMoniker = null;
            }

            while (m_Objects.Count > 0)
            {
                var _moniker = m_Objects[0];
                _moniker.Dispose();
                m_Objects.RemoveAt(0);
            }

            base.Dispose();
        }

        #endregion

        #region Variabless

        protected List<T> m_Objects = new List<T>();
        protected IEnumMoniker m_pEnumMoniker;
        protected Guid m_Category = Guid.Empty;

        #endregion

        #region Properties

        public Guid Category => m_Category;

        public T this[int index] => m_Objects[index];

        public T this[string name]
        {
            get
            {
                foreach (var _info in m_Objects)
                    if (_info.Name == name || _info.DevicePath == name)
                        return _info;
                return null;
            }
        }

        public T this[Guid _clsid]
        {
            get
            {
                foreach (var _info in m_Objects)
                    if (_info.ClassID == _clsid)
                        return _info;
                return null;
            }
        }

        public int Count => m_Objects.Count;

        public List<T> Objects => m_Objects;

        #endregion
    }

    #endregion

    #region Device Categories

    [ComVisible(false)]
    public class DSDevice : MonikerInfo
    {
        #region Constructor

        #endregion

        #region Fields

        public DSFilter Filter
        {
            get
            {
                DSFilter _filter = null;
                if (m_pUnknown != null)
                {
                    _filter = new DSFilter(DevicePath);
                    if (_filter.IsReleased)
                    {
                        _filter.Dispose();
                        _filter = null;
                    }
                }

                return _filter;
            }
        }

        #endregion

        #region Overridden Methods

        public override string ToString()
        {
            var _value = Name;
            if (_value != null && _value != "") return _value;
            _value = DevicePath;
            if (_value != null && _value != "") return _value;
            return base.ToString();
        }

        #endregion
    }

    [ComVisible(false)]
    public class DSCategory : DSEnumerator<DSDevice>
    {
        #region Constructor

        public DSCategory(Guid _category)
            : base(_category)
        {
        }

        #endregion

        #region Properties

        public new DSFilter this[int index]
        {
            get
            {
                var _device = base[index];
                if (_device) return _device.Filter;
                return null;
            }
        }

        public new DSFilter this[string name]
        {
            get
            {
                var _device = base[name];
                if (_device) return _device.Filter;
                return null;
            }
        }

        public new DSFilter this[Guid _clsid]
        {
            get
            {
                var _device = base[_clsid];
                if (_device) return _device.Filter;
                return null;
            }
        }

        #endregion
    }

    #endregion

    #region Pin Helper

    [ComVisible(false)]
    public class DSPin : DSObject<IPin>
    {
        #region Overriden Methods

        public override string ToString()
        {
            return " { " + base.ToString() + " } Name: '" + Name + "' ( " + Direction + " )" +
                   (IsConnected ? " Connected" : " Not Connected");
            ;
        }

        #endregion

        #region Constructor

        public DSPin(IntPtr _pin)
            : base(_pin)
        {
        }

        public DSPin(IPin _pin)
            : base(_pin)
        {
        }

        #endregion

        #region Properties

        public PinDirection Direction
        {
            get
            {
                PinDirection _direction;
                m_pUnknown.QueryDirection(out _direction);
                return _direction;
            }
        }

        public bool IsConnected => ConnectedTo != null && ConnectedTo == true;

        public string Name
        {
            get
            {
                if (this)
                {
                    PinInfo _info;
                    if (SUCCEEDED(m_pUnknown.QueryPinInfo(out _info)))
                    {
                        _info.filter = null;
                        return _info.name;
                    }
                }

                return "";
            }
        }

        public DSFilter Filter
        {
            get
            {
                if (this)
                {
                    PinInfo _info;
                    if (SUCCEEDED(m_pUnknown.QueryPinInfo(out _info))) return new DSFilter(_info.filter);
                }

                return null;
            }
        }

        public DSPin ConnectedTo
        {
            get
            {
                if (this)
                {
                    IntPtr pConnected;
                    if (SUCCEEDED(m_pUnknown.ConnectedTo(out pConnected))) return new DSPin(pConnected);
                }

                return null;
            }
        }

        public AMMediaType ConnectionMediaType
        {
            get
            {
                if (this)
                {
                    var mt = new AMMediaType();
                    if (SUCCEEDED(m_pUnknown.ConnectionMediaType(mt))) return mt;
                    mt.Free();
                }

                return null;
            }
        }

        public AMMediaType Format
        {
            get
            {
                if (this)
                {
                    var _config = (IAMStreamConfig)QueryInterface(typeof(IAMStreamConfig));
                    if (_config != null)
                    {
                        AMMediaType mt;
                        if (SUCCEEDED(_config.GetFormat(out mt))) return mt;
                    }
                }

                return null;
            }
            set
            {
                if (this)
                {
                    var _config = (IAMStreamConfig)QueryInterface(typeof(IAMStreamConfig));
                    if (_config != null)
                    {
                        var hr = _config.SetFormat(value);
                        ASSERT(SUCCEEDED(hr));
                    }
                }
            }
        }

        public List<AMMediaType> MediaTypes
        {
            get
            {
                var _list = new List<AMMediaType>();
                if (this)
                {
                    IEnumMediaTypes pEnum;
                    IntPtr _ptr;
                    if (SUCCEEDED(m_pUnknown.EnumMediaTypes(out _ptr)))
                    {
                        pEnum = (IEnumMediaTypes)Marshal.GetObjectForIUnknown(_ptr);
                        var ulMediaCount = Marshal.AllocCoTaskMem(Marshal.SizeOf(typeof(uint)));
                        var pTypes = Marshal.AllocCoTaskMem(Marshal.SizeOf(typeof(IntPtr)));

                        try
                        {
                            do
                            {
                                var hr = pEnum.Next(1, pTypes, ulMediaCount);
                                if (hr != S_OK) break;
                                ASSERT(Marshal.ReadInt32(ulMediaCount) == 1);
                                var _ptrStructure = Marshal.ReadIntPtr(pTypes);
                                try
                                {
                                    if (_ptrStructure != IntPtr.Zero)
                                        _list.Add((AMMediaType)Marshal.PtrToStructure(_ptrStructure,
                                            typeof(AMMediaType)));
                                }
                                finally
                                {
                                    if (_ptrStructure != IntPtr.Zero) Marshal.FreeCoTaskMem(_ptrStructure);
                                }
                            } while (true);
                        }
                        finally
                        {
                            Marshal.FreeCoTaskMem(ulMediaCount);
                            Marshal.FreeCoTaskMem(pTypes);
                        }
                    }
                }

                return _list;
            }
        }

        public Guid Category
        {
            get
            {
                var _result = Guid.Empty;
                var iSize = Marshal.SizeOf(typeof(Guid));
                var ipOut = Marshal.AllocCoTaskMem(iSize);
                try
                {
                    int cbBytes;

                    var pKs = (IKsPropertySet)QueryInterface(typeof(IKsPropertySet));
                    if (pKs != null)
                    {
                        var g = PropSetID.Pin;
                        var hr = pKs.Get(g, (int)AMPropertyPin.Category, IntPtr.Zero, 0, ipOut, iSize, out cbBytes);
                        if (SUCCEEDED(hr)) _result = (Guid)Marshal.PtrToStructure(ipOut, typeof(Guid));
                    }
                }
                finally
                {
                    Marshal.FreeCoTaskMem(ipOut);
                    ipOut = IntPtr.Zero;
                }

                return _result;
            }
        }

        public AMMediaType this[int index]
        {
            get
            {
                try
                {
                    return MediaTypes[index];
                }
                catch
                {
                    return null;
                }
            }
        }

        public int Count => MediaTypes.Count;

        #endregion

        #region Methods

        public HRESULT ConnectDirect(DSPin _pin)
        {
            try
            {
                if (!_pin) return Render();
                if (!this) return E_POINTER;
                var _graph = Filter.FilterGraph;
                if (_graph != null)
                {
                    if (Direction != _pin.Direction)
                    {
                        if (Direction == PinDirection.Input)
                            return (HRESULT)_graph.ConnectDirect(_pin.m_pUnknown, m_pUnknown, null);
                        return (HRESULT)_graph.ConnectDirect(m_pUnknown, _pin.m_pUnknown, null);
                    }

                    return VFW_E_INVALID_DIRECTION;
                }

                return VFW_E_NOT_IN_GRAPH;
            }
            catch (Exception _exception)
            {
                return (HRESULT)Marshal.GetHRForException(_exception);
            }
        }

        public HRESULT Connect(DSPin _pin)
        {
            try
            {
                var hr = ConnectDirect(_pin);
                if (hr.Succeeded) return hr;
                if (!_pin) return Render();
                if (!this) return E_POINTER;
                var _graph = Filter.FilterGraph;
                if (_graph != null)
                {
                    if (Direction != _pin.Direction)
                    {
                        if (Direction == PinDirection.Input)
                            return (HRESULT)_graph.Connect(_pin.m_pUnknown, m_pUnknown);
                        return (HRESULT)_graph.Connect(m_pUnknown, _pin.m_pUnknown);
                    }

                    return VFW_E_INVALID_DIRECTION;
                }

                return VFW_E_NOT_IN_GRAPH;
            }
            catch (Exception _exception)
            {
                return (HRESULT)Marshal.GetHRForException(_exception);
            }
        }

        public HRESULT Disconnect()
        {
            DSPin _pin = null;
            try
            {
                if (!this) return E_POINTER;
                var _graph = Filter.FilterGraph;
                if (_graph != null)
                {
                    _pin = ConnectedTo;
                    if (!_pin) return VFW_E_NOT_CONNECTED;
                    var hr = _graph.Disconnect(m_pUnknown);
                    if (SUCCEEDED(hr)) return (HRESULT)_graph.Disconnect(_pin.m_pUnknown);
                    return (HRESULT)hr;
                }

                return VFW_E_NOT_IN_GRAPH;
            }
            catch (Exception _exception)
            {
                return (HRESULT)Marshal.GetHRForException(_exception);
            }
            finally
            {
                if (_pin)
                {
                    _pin.Dispose();
                    _pin = null;
                }
            }
        }

        public HRESULT Render()
        {
            try
            {
                if (!this) return E_POINTER;
                var _graph = Filter.FilterGraph;
                if (_graph != null) return (HRESULT)_graph.Render(m_pUnknown);
                return VFW_E_NOT_IN_GRAPH;
            }
            catch (Exception _exception)
            {
                return (HRESULT)Marshal.GetHRForException(_exception);
            }
        }

        public HRESULT IsAccepted(AMMediaType mt)
        {
            if (this) return (HRESULT)m_pUnknown.QueryAccept(mt);
            return E_FAIL;
        }

        public DSPin RemoveFiltersChain()
        {
            var _connected = ConnectedTo;
            if (_connected != null && _connected)
            {
                var _type = ConnectionMediaType;
                Disconnect();
                var _pins = _connected.Filter.Pins;
                var _disconnect = new List<DSPin>();
                var bShouldProceed = true;
                foreach (var _pin in _pins)
                    if (_pin.IsConnected)
                    {
                        if (_pin.Direction != Direction)
                        {
                            bShouldProceed = false;
                            break;
                        }

                        var mt = _pin.ConnectionMediaType;
                        if (mt.majorType != _type.majorType)
                        {
                            bShouldProceed = false;
                            break;
                        }

                        _disconnect.Add(_pin);
                    }

                if (bShouldProceed)
                {
                    DSPin _result = null;
                    foreach (var _pin in _disconnect) _result = _pin.RemoveFiltersChain();
                    var _filter = _connected.Filter;
                    _filter.FilterGraph = null;
                    _filter.Dispose();
                    if (Direction == PinDirection.Output)
                        _connected = this;
                    else
                        _connected = _result;
                }
            }

            return _connected;
        }

        #endregion
    }

    #endregion

    #region Filter Helper

    [ComVisible(false)]
    public class DSFilter : DSObject<IBaseFilter>
    {
        #region Overriden Methods

        public override string ToString()
        {
            return Name;
        }

        #endregion

        #region Constructor

        protected DSFilter()
        {
        }

        public DSFilter(IBaseFilter _filter)
            : base(_filter)
        {
        }

        public DSFilter(Guid _filter)
            : base(_filter)
        {
        }

        public DSFilter(string _moniker)
        {
            var _info = new MonikerInfo(_moniker);
            m_pUnknown = (IBaseFilter)_info.Bind(typeof(IBaseFilter));
        }

        #endregion

        #region Properties

        public List<DSPin> Pins
        {
            get
            {
                var _pins = new List<DSPin>();
                if (this)
                {
                    var pPins = new IPin[1];
                    IEnumPins pEnum;
                    if (SUCCEEDED(m_pUnknown.EnumPins(out pEnum)))
                        while (pEnum.Next(1, pPins, IntPtr.Zero) == S_OK)
                            if (pPins[0] != null)
                                _pins.Add(new DSPin(pPins[0]));
                }

                return _pins;
            }
        }

        public List<DSPin> Output
        {
            get
            {
                var _pins = Pins;
                var _output = new List<DSPin>();
                foreach (var _pin in _pins)
                    if (_pin.Direction == PinDirection.Output)
                        _output.Add(_pin);
                return _output;
            }
        }

        public List<DSPin> Input
        {
            get
            {
                var _pins = Pins;
                var _output = new List<DSPin>();
                foreach (var _pin in _pins)
                    if (_pin.Direction == PinDirection.Input)
                        _output.Add(_pin);
                return _output;
            }
        }

        public string Name
        {
            get
            {
                if (this)
                {
                    FilterInfo _info;
                    if (SUCCEEDED(m_pUnknown.QueryFilterInfo(out _info)))
                    {
                        if (_info.pGraph != null) Marshal.ReleaseComObject(_info.pGraph);
                        return _info.achName;
                    }
                }

                return "";
            }
        }

        public bool HaveProperties => IsSupported(typeof(ISpecifyPropertyPages));

        public IGraphBuilder FilterGraph
        {
            get
            {
                IGraphBuilder _graph = null;
                if (this)
                {
                    FilterInfo _info;
                    if (SUCCEEDED(m_pUnknown.QueryFilterInfo(out _info)))
                        if (_info.pGraph != null)
                        {
                            _graph = (IGraphBuilder)_info.pGraph;
                            Marshal.ReleaseComObject(_info.pGraph);
                        }
                }

                return _graph;
            }
            set
            {
                if (this)
                {
                    if (value == null)
                    {
                        FilterInfo _info;
                        if (SUCCEEDED(m_pUnknown.QueryFilterInfo(out _info)))
                            if (_info.pGraph != null)
                            {
                                var _graph = (IGraphBuilder)_info.pGraph;
                                _graph.RemoveFilter(m_pUnknown);
                                Marshal.ReleaseComObject(_info.pGraph);
                            }
                    }
                    else
                    {
                        var hr = value.AddFilter(m_pUnknown, Name);
                        ASSERT(SUCCEEDED(hr));
                    }
                }
            }
        }

        public DSPin InputPin
        {
            get
            {
                var _pins = Input;
                if (_pins.Count > 0)
                    return _pins[0];
                return null;
            }
        }

        public DSPin OutputPin
        {
            get
            {
                var _pins = Output;
                if (_pins.Count > 0)
                    return _pins[0];
                return null;
            }
        }

        public DSPin this[int index] => Pins[index];

        public DSPin this[string _name]
        {
            get
            {
                var _pins = Pins;
                foreach (var _pin in _pins)
                    if (_pin.Name == _name)
                        return _pin;
                return null;
            }
        }

        public int Count => Pins.Count;

        #endregion

        #region Public Methods

        public HRESULT ShowProperties()
        {
            return ShowProperties(IntPtr.Zero);
        }

        public HRESULT ShowProperties(IntPtr _hwnd)
        {
            if (BasePropertyPage.ShowPropertyPages(m_pUnknown, _hwnd)) return S_OK;
            return E_FAIL;
        }

        public DSPin GetPin(PinDirection _direction)
        {
            return GetPin(_direction, 0);
        }

        public DSPin GetPin(PinDirection _direction, int index)
        {
            List<DSPin> _pins = null;
            switch (_direction)
            {
                case PinDirection.Input:
                    _pins = Input;
                    break;
                case PinDirection.Output:
                    _pins = Output;
                    break;
            }

            ASSERT(_pins);
            if (index >= 0 && _pins.Count < index) return _pins[index];
            return null;
        }

        public DSPin GetPin(string _name)
        {
            var _pins = Pins;
            foreach (var _pin in _pins)
                if (_pin.Name == _name)
                    return _pin;
            return null;
        }

        public DSPin GetPin(Guid _category)
        {
            var _pins = Pins;
            foreach (var _pin in _pins)
                if (_pin.Category == _category)
                    return _pin;
            return null;
        }

        public HRESULT Connect(DSFilter _filter)
        {
            if (_filter)
            {
                List<DSPin> _pins;
                {
                    _pins = _filter.Input;
                    foreach (var _pin in _pins)
                    {
                        var hr = Connect(_pin);
                        if (hr.Succeeded) return hr;
                    }
                }
                {
                    _pins = _filter.Output;
                    foreach (var _pin in _pins)
                    {
                        var hr = Connect(_pin);
                        if (hr.Succeeded) return hr;
                    }
                }
                return VFW_E_CANNOT_CONNECT;
            }

            return E_POINTER;
        }

        public HRESULT Connect(DSPin _pin)
        {
            if (_pin)
            {
                List<DSPin> _pins;
                if (_pin.Direction == PinDirection.Output)
                    _pins = Input;
                else
                    _pins = Output;
                foreach (var _connect in _pins)
                    if (!_connect.IsConnected)
                    {
                        var hr = _connect.Connect(_pin);
                        if (hr.Succeeded) return hr;
                    }

                return VFW_E_CANNOT_CONNECT;
            }

            return E_POINTER;
        }

        public DSPin RemoveFiltersChain()
        {
            DSPin _input = null;
            DSPin _output = null;
            _output = RemoveFiltersChain(PinDirection.Output);
            _input = RemoveFiltersChain(PinDirection.Input);
            if (_output != null) return _output;
            return _input;
        }

        public DSPin RemoveFiltersChain(PinDirection _direction)
        {
            List<DSPin> _pins;
            if (_direction == PinDirection.Input)
                _pins = Input;
            else
                _pins = Output;
            DSPin _result = null;
            foreach (var _pin in _pins)
            {
                var _removed = _pin.RemoveFiltersChain();
                if (_removed != null && _result == null) _result = _removed;
            }

            return _result;
        }

        #endregion
    }

    [ComVisible(false)]
    public class DSBaseSourceFilter : DSFilter
    {
        #region Properties

        public string FileName
        {
            get
            {
                var _source = (IFileSourceFilter)QueryInterface(typeof(IFileSourceFilter));
                if (_source != null)
                {
                    string _path;
                    var hr = _source.GetCurFile(out _path, null);
                    if (SUCCEEDED(hr)) return _path;
                }

                return null;
            }
            set
            {
                var _source = (IFileSourceFilter)QueryInterface(typeof(IFileSourceFilter));
                if (_source != null)
                {
                    var hr = _source.Load(value, null);
                    ASSERT(SUCCEEDED(hr));
                }
            }
        }

        #endregion

        #region Constructor

        protected DSBaseSourceFilter()
        {
        }

        public DSBaseSourceFilter(IBaseFilter _filter)
            : base(_filter)
        {
        }

        public DSBaseSourceFilter(Guid _filter)
            : base(_filter)
        {
        }

        public DSBaseSourceFilter(string _moniker)
            : base(_moniker)
        {
        }

        #endregion
    }

    [ComVisible(false)]
    public class DSBaseWriterFilter : DSFilter
    {
        #region Properties

        public string FileName
        {
            get
            {
                var _sink = (IFileSinkFilter)QueryInterface(typeof(IFileSinkFilter));
                if (_sink != null)
                {
                    string _path;
                    var hr = _sink.GetCurFile(out _path, null);
                    if (SUCCEEDED(hr)) return _path;
                }

                return null;
            }
            set
            {
                var _sink = (IFileSinkFilter)QueryInterface(typeof(IFileSinkFilter));
                if (_sink != null)
                {
                    var hr = _sink.SetFileName(value, null);
                    ASSERT(SUCCEEDED(hr));
                }
            }
        }

        #endregion

        #region Constructor

        protected DSBaseWriterFilter()
        {
        }

        public DSBaseWriterFilter(IBaseFilter _filter)
            : base(_filter)
        {
        }

        public DSBaseWriterFilter(Guid _filter)
            : base(_filter)
        {
        }

        public DSBaseWriterFilter(string _moniker)
            : base(_moniker)
        {
        }

        #endregion
    }

    #endregion

    #region Categories

    [ComVisible(false)]
    public class DSVideoCaptureCategory : DSCategory
    {
        #region Constructor

        public DSVideoCaptureCategory()
            : base(FilterCategory.VideoInputDevice)
        {
        }

        #endregion
    }

    [ComVisible(false)]
    public class DSAudioCaptureCategory : DSCategory
    {
        #region Constructor

        public DSAudioCaptureCategory()
            : base(FilterCategory.AudioInputDevice)
        {
        }

        #endregion
    }

    [ComVisible(false)]
    public class DSVideoCompressorsCategory : DSCategory
    {
        #region Constructor

        public DSVideoCompressorsCategory()
            : base(FilterCategory.VideoCompressorCategory)
        {
        }

        #endregion
    }

    [ComVisible(false)]
    public class DSAudioCompressorsCategory : DSCategory
    {
        #region Constructor

        public DSAudioCompressorsCategory()
            : base(FilterCategory.AudioCompressorCategory)
        {
        }

        #endregion
    }

    [ComVisible(false)]
    public class DSAudioRenderersCategory : DSCategory
    {
        #region Constructor

        public DSAudioRenderersCategory()
            : base(FilterCategory.AudioRendererCategory)
        {
        }

        #endregion
    }

    #endregion

    #region Filters

    [ComVisible(false)]
    [Guid("f8388a40-d5bb-11d0-be5a-0080c706568e")]
    public class DSInfTeeFilter : DSFilter
    {
    }

    [ComVisible(false)]
    [Guid("cc58e280-8aa1-11d1-b3f1-00aa003761c5")]
    public class DSSmartTeeFilter : DSFilter
    {
    }

    [ComVisible(false)]
    [Guid("8596e5f0-0da5-11d0-bd21-00a0c911ce86")]
    public class DSFileWriter : DSBaseWriterFilter
    {
    }

    [ComVisible(false)]
    [Guid("e436ebb5-524f-11ce-9f53-0020af0ba770")]
    public class DSFileSourceAsync : DSBaseSourceFilter
    {
    }

    [ComVisible(false)]
    [Guid("79376820-07d0-11cf-a24d-0020afd79767")]
    public class DSDSoundRendererFilter : DSFilter
    {
    }

    [ComVisible(false)]
    [Guid("70e102b0-5556-11ce-97c0-00aa0055595a")]
    public class DSVideoRenderer : DSFilter
    {
    }

    [ComVisible(false)]
    [Guid("c1f400a4-3f08-11d3-9f0b-006008039e37")]
    public class DSNullRenderer : DSFilter
    {
    }

    [ComVisible(false)]
    [Guid("c1f400a0-3f08-11d3-9f0b-006008039e37")]
    public class DSSampleGrabberFilter : DSFilter, ISampleGrabberCB
    {
        #region Delegates

        [ComVisible(false)]
        public delegate void SampleEventHandler(DSSampleGrabberFilter _filter, IntPtr _buffer, int _length);

        #endregion

        #region Constructor

        public DSSampleGrabberFilter()
        {
            m_SampleGrabber = (ISampleGrabber)QueryInterface(typeof(ISampleGrabber));
            ASSERT(m_SampleGrabber);
            if (m_SampleGrabber != null)
            {
                var hr = (HRESULT)m_SampleGrabber.SetCallback(this, 1);
                hr.Assert();
                hr = (HRESULT)m_SampleGrabber.SetBufferSamples(m_BufferedSamples);
                hr.Assert();
                hr = (HRESULT)m_SampleGrabber.SetOneShot(m_bOneShot);
                hr.Assert();
            }
        }

        #endregion

        #region Events

        public event SampleEventHandler OnSample;

        #endregion

        #region Overridden Methods

        public override void Dispose()
        {
            m_SampleGrabber = (ISampleGrabber)QueryInterface(typeof(ISampleGrabber));
            if (m_SampleGrabber != null)
            {
                m_SampleGrabber.SetCallback(null, 0);
                m_SampleGrabber = null;
            }

            base.Dispose();
        }

        #endregion

        #region Variables

        protected ISampleGrabber m_SampleGrabber;
        protected bool m_BufferedSamples;
        protected bool m_bOneShot;

        #endregion

        #region Properties

        public AMMediaType MediaType
        {
            get
            {
                if (m_SampleGrabber != null)
                {
                    var mt = new AMMediaType();
                    if (SUCCEEDED(m_SampleGrabber.GetConnectedMediaType(mt))) return mt;
                    mt.Free();
                }

                return null;
            }
            set
            {
                if (m_SampleGrabber != null)
                {
                    var hr = (HRESULT)m_SampleGrabber.SetMediaType(value);
                    hr.Assert();
                }
            }
        }

        public bool BufferedSamples
        {
            get => m_BufferedSamples;
            set
            {
                if (m_BufferedSamples != value)
                {
                    var hr = NOERROR;
                    if (m_SampleGrabber != null)
                    {
                        hr = (HRESULT)m_SampleGrabber.SetBufferSamples(value);
                        hr.Assert();
                    }

                    if (hr.Succeeded) m_BufferedSamples = value;
                }
            }
        }

        public bool OneShot
        {
            get => m_bOneShot;
            set
            {
                if (m_bOneShot != value)
                {
                    var hr = NOERROR;
                    if (m_SampleGrabber != null)
                    {
                        hr = (HRESULT)m_SampleGrabber.SetOneShot(value);
                        hr.Assert();
                    }

                    if (hr.Succeeded) m_bOneShot = value;
                }
            }
        }

        #endregion

        #region ISampleGrabberCB Members

        public int SampleCB(double _time, IntPtr pSample)
        {
            return NOERROR;
        }

        public int BufferCB(double _time, IntPtr pBuffer, int _length)
        {
            if (OnSample != null) OnSample(this, pBuffer, _length);
            return NOERROR;
        }

        #endregion
    }

    [ComVisible(false)]
    [Guid("7c23220e-55bb-11d3-8b16-00c04fb6bd3d")]
    public class DSWMAsfWritter : DSBaseWriterFilter
    {
    }

    [ComVisible(false)]
    [Guid("187463A0-5BB7-11d3-ACBE-0080C75E246E")]
    public class DSWMAsfReader : DSBaseSourceFilter
    {
    }

    [ComVisible(false)]
    [Guid("E2510970-F137-11CE-8B67-00AA00A3F1A6")]
    public class DSAviMuxFilter : DSFilter
    {
    }

    #endregion

    #region Classes

    [ComVisible(false)]
    [Guid("e436ebb3-524f-11ce-9f53-0020af0ba770")]
    public class DSGraphBuilder : DSObject<IGraphBuilder>
    {
    }

    [ComVisible(false)]
    [Guid("BF87B6E1-8C27-11d0-B3F0-00AA003761C5")]
    public class DSCaptureGraphBuilder2 : DSObject<ICaptureGraphBuilder2>
    {
    }

    #endregion

    #region Base Filter Graph Builder

    [ComVisible(false)]
    [SecurityPermission(SecurityAction.InheritanceDemand, UnmanagedCode = true)]
    [SecurityPermission(SecurityAction.Demand, UnmanagedCode = true)]
    public class DSFilterGraphBase : COMHelper, IDisposable
    {
        #region Constants

        protected const int WM_GRAPHNOTIFY = 0x00008001;

        #endregion

        #region IDisposable Members

        public virtual void Dispose()
        {
            CloseInterfaces();
        }

        #endregion

        #region Helper Classes

        [ComVisible(false)]
        protected class HelperForm : Form
        {
            protected override void WndProc(ref Message m)
            {
                if (m.Msg == WM_GRAPHNOTIFY)
                    try
                    {
                        if (m.LParam != IntPtr.Zero)
                        {
                            var _graph = (DSFilterGraphBase)Marshal.GetObjectForIUnknown(m.LParam);
                            if (_graph != null)
                                if (_graph.ProcessGraphMessage())
                                    _graph.Stop();
                        }
                    }
                    catch
                    {
                    }

                base.WndProc(ref m);
            }
        }

        #endregion

        #region Variables

        protected Control m_VideoControl;
        protected Control m_EventControl = new HelperForm();
        protected IGraphBuilder m_GraphBuilder;
        protected IMediaControl m_MediaControl;
        protected IMediaSeeking m_MediaSeeking;
        protected IMediaEventEx m_MediaEventEx;
        protected IVideoWindow m_VideoWindow;
        protected IBasicAudio m_BasicAudio;
        protected IBasicVideo m_BasicVideo;
        protected IVideoFrameStep m_FrameStep;

        protected bool m_bMute;
        protected int m_iVolume;
        protected double m_dRate = 1.0;
        protected bool m_bShouldPreview = true;

        protected List<DSFilter> m_Filters = new List<DSFilter>();

        #endregion

        #region Constructor

        ~DSFilterGraphBase()
        {
            Dispose();
        }

        #endregion

        #region Properties

        public bool IsRunning
        {
            get
            {
                if (m_MediaControl != null)
                {
                    FilterState _state;
                    var hr = 0;
                    do
                    {
                        hr = m_MediaControl.GetState(200, out _state);
                    } while (hr == 0x00040237 || hr == 0x00040268);

                    if (hr == 0) return _state == FilterState.Running;
                }

                return false;
            }
        }

        public bool IsPaused
        {
            get
            {
                if (m_MediaControl != null)
                {
                    FilterState _state;
                    var hr = 0;
                    do
                    {
                        hr = m_MediaControl.GetState(200, out _state);
                    } while (hr == 0x00040237 || hr == 0x00040268);

                    if (hr == 0) return _state == FilterState.Paused;
                }

                return false;
            }
        }

        public bool IsStopped
        {
            get
            {
                if (m_MediaControl != null)
                {
                    FilterState _state;
                    var hr = 0;
                    do
                    {
                        hr = m_MediaControl.GetState(200, out _state);
                    } while (hr == 0x00040237 || hr == 0x00040268);

                    if (hr == 0) return _state == FilterState.Stopped;
                }

                return true;
            }
        }

        public long Position
        {
            get
            {
                if (m_MediaSeeking != null)
                {
                    long _time;
                    var hr = m_MediaSeeking.GetCurrentPosition(out _time);
                    Debug.Assert(hr == 0);
                    if (hr == 0) return _time;
                }

                return -1;
            }
            set
            {
                if (m_MediaSeeking != null)
                {
                    DsLong _stop = (long)0;
                    DsLong _current = value;
                    var hr = m_MediaSeeking.SetPositions(_current, AMSeekingSeekingFlags.AbsolutePositioning, _stop,
                        AMSeekingSeekingFlags.NoPositioning);
                    Debug.Assert(hr >= 0);
                    if (OnPositionChange != null) OnPositionChange(this, EventArgs.Empty);
                }
            }
        }

        public long Duration
        {
            get
            {
                if (m_MediaSeeking != null)
                {
                    long _time;
                    var hr = m_MediaSeeking.GetDuration(out _time);
                    Debug.Assert(hr == 0);
                    if (hr == 0) return _time;
                }

                return -1;
            }
        }

        public int Volume
        {
            get => m_iVolume;
            set
            {
                m_iVolume = value;
                if (!m_bMute) SetVolume(value);
            }
        }

        public bool Mute
        {
            get => m_bMute;
            set
            {
                if (m_bMute != value)
                {
                    m_bMute = value;
                    SetVolume(m_bMute ? -10000 : m_iVolume);
                }
            }
        }

        public double Rate
        {
            get => m_dRate;
            set
            {
                if (value > 0 && value <= 2.0)
                {
                    if (m_MediaSeeking != null)
                    {
                        var hr = m_MediaSeeking.SetRate(value);
                        if (hr == 0) m_dRate = value;
                    }
                    else
                    {
                        m_dRate = value;
                    }
                }
            }
        }

        public Control VideoControl
        {
            get => m_VideoControl;
            set
            {
                if (m_VideoControl != value)
                {
                    m_VideoControl = value;
                    m_VideoControl.Resize += VideoControl_Resize;
                    m_VideoControl.VisibleChanged += VideoControl_VisibleChanged;
                }
            }
        }

        public bool Visible
        {
            get
            {
                var bVisible = m_VideoControl != null ? m_VideoControl.Visible : false;
                return m_bShouldPreview || bVisible;
            }
            set
            {
                if (m_bShouldPreview != value)
                {
                    m_bShouldPreview = value;
                    if (m_VideoWindow != null)
                    {
                        m_VideoWindow.put_AutoShow(m_bShouldPreview ? -1 : 0);
                        m_VideoWindow.put_Visible(m_bShouldPreview ? -1 : 0);
                    }
                }
            }
        }

        public List<DSFilter> Filters
        {
            get
            {
                if (m_MediaControl == null)
                {
                    while (m_Filters.Count > 0)
                    {
                        var _filter = m_Filters[0];
                        m_Filters.RemoveAt(0);
                        _filter.Dispose();
                    }

                    if (m_GraphBuilder != null)
                    {
                        IEnumFilters pEnum;
                        if (SUCCEEDED(m_GraphBuilder.EnumFilters(out pEnum)))
                        {
                            var aFilters = new IBaseFilter[1];
                            while (S_OK == pEnum.Next(1, aFilters, IntPtr.Zero))
                                m_Filters.Add(new DSFilter(aFilters[0]));
                            Marshal.ReleaseComObject(pEnum);
                        }
                    }
                }

                return m_Filters;
            }
        }

        public int Count => Filters.Count;

        public DSFilter AudioRenderer
        {
            get
            {
                var _filters = Filters;
                foreach (var _filter in _filters)
                    if (_filter.IsSupported(typeof(IBasicAudio).GUID))
                        return _filter;
                return null;
            }
        }

        public DSFilter VideoRenderer
        {
            get
            {
                var _filters = Filters;
                foreach (var _filter in _filters)
                    if (_filter.IsSupported(typeof(IVideoWindow)))
                        return _filter;
                return null;
            }
        }

        public DSFilter this[int index]
        {
            get
            {
                try
                {
                    return Filters[index];
                }
                catch
                {
                    return null;
                }
            }
        }

        public DSFilter this[string _name]
        {
            get
            {
                var _filters = Filters;
                foreach (var _filter in _filters)
                    if (_filter.Name == _name)
                        return _filter;
                return null;
            }
        }

        public bool IsAudioSupported => AudioRenderer != null && AudioRenderer == true;

        #endregion

        #region Events

        public event EventHandler OnPlaybackPrepared;
        public event EventHandler OnPlaybackStart;
        public event EventHandler OnPlaybackStop;
        public event EventHandler OnPlaybackPause;
        public event EventHandler OnPositionChange;
        public event EventHandler OnPlaybackReady;

        #endregion

        #region Public Methods

        public virtual HRESULT Start()
        {
            var hr = 0;
            if (m_MediaControl == null)
            {
                hr = Load();
                if (hr != 0) return (HRESULT)hr;
            }

            hr = 1;
            while (hr == 1)
            {
                hr = m_MediaControl.Run();
                if (hr == 1) Thread.Sleep(50);
            }

            if (hr == 0)
                if (OnPlaybackStart != null)
                    OnPlaybackStart(this, EventArgs.Empty);
            return (HRESULT)hr;
        }

        public virtual HRESULT Pause()
        {
            var hr = 0;
            if (m_MediaControl == null)
            {
                hr = Load();
                if (hr != 0) return (HRESULT)hr;
            }

            if (!IsPaused)
                hr = m_MediaControl.Pause();
            else
                return Start();
            if (hr >= 0)
                if (OnPlaybackPause != null)
                    OnPlaybackPause(this, EventArgs.Empty);
            return (HRESULT)hr;
        }

        public virtual HRESULT Stop()
        {
            if (m_MediaControl == null) return E_POINTER;
            m_MediaControl.Stop();
            if (OnPlaybackStop != null) OnPlaybackStop(this, EventArgs.Empty);
            return Unload();
        }

        public virtual HRESULT StepForward()
        {
            if (m_FrameStep != null)
            {
                if (!IsPaused) Pause();
                var hr = m_FrameStep.Step(1, null);
                if (hr < 0)
                {
                    long _time;
                    hr = m_MediaSeeking.GetCurrentPosition(out _time);
                    DsLong _stop = (long)0;
                    var _ts = new TimeSpan(0, 0, 1);
                    _time += _ts.Ticks / 20;
                    DsLong _current = _time;
                    hr = m_MediaSeeking.SetPositions(_current, AMSeekingSeekingFlags.AbsolutePositioning, _stop,
                        AMSeekingSeekingFlags.NoPositioning);
                }

                return (HRESULT)hr;
            }

            return E_POINTER;
        }

        public virtual HRESULT StepBackward()
        {
            if (m_FrameStep != null)
            {
                if (!IsPaused) Pause();
                long _time;
                var hr = m_MediaSeeking.GetCurrentPosition(out _time);
                DsLong _stop = (long)0;
                var _ts = new TimeSpan(0, 0, 1);
                _time -= _ts.Ticks / 20;
                if (_time < 0) _time = 0;
                DsLong _current = _time;
                hr = m_MediaSeeking.SetPositions(_current, AMSeekingSeekingFlags.AbsolutePositioning, _stop,
                    AMSeekingSeekingFlags.NoPositioning);
                return (HRESULT)hr;
            }

            return E_POINTER;
        }

        #endregion

        #region Private Methods

        private void SetVolume(int _volume)
        {
            if (m_BasicAudio == null)
            {
                if (m_GraphBuilder == null)
                    return;
                m_BasicAudio = (IBasicAudio)m_GraphBuilder;
            }

            m_BasicAudio.put_Volume(_volume);
        }

        private HRESULT InitInterfaces()
        {
            CloseInterfaces();
            var hr = E_FAIL;
            try
            {
                m_GraphBuilder = (IGraphBuilder)new FilterGraph();
                hr = OnInitInterfaces();
                hr.Throw();
                return PreparePlayback();
            }
            catch (Exception _exception)
            {
                if (_exception is COMException)
                    hr = (HRESULT)((COMException)_exception).ErrorCode;
                else
                    hr = E_UNEXPECTED;
            }
            finally
            {
                if (hr.Succeeded)
                {
                    while (m_Filters.Count > 0)
                    {
                        var _filter = m_Filters[0];
                        m_Filters.RemoveAt(0);
                        _filter.Dispose();
                    }

                    IEnumFilters pEnum;
                    if (SUCCEEDED(m_GraphBuilder.EnumFilters(out pEnum)))
                    {
                        var aFilters = new IBaseFilter[1];
                        while (S_OK == pEnum.Next(1, aFilters, IntPtr.Zero)) m_Filters.Add(new DSFilter(aFilters[0]));
                        Marshal.ReleaseComObject(pEnum);
                    }
                }
            }

            CloseInterfaces();
            return hr;
        }

        private HRESULT CloseInterfaces()
        {
            try
            {
                OnCloseInterfaces();
                if (m_MediaEventEx != null)
                {
                    m_MediaEventEx.SetNotifyWindow(IntPtr.Zero, 0, IntPtr.Zero);
                    m_MediaEventEx = null;
                }

                if (m_VideoWindow != null)
                {
                    m_VideoWindow.put_Visible(0);
                    m_VideoWindow.put_Owner(IntPtr.Zero);
                    m_VideoWindow = null;
                }

                m_MediaSeeking = null;
                m_BasicVideo = null;
                m_BasicAudio = null;
                m_MediaControl = null;
                while (m_Filters.Count > 0)
                {
                    var _filter = m_Filters[0];
                    m_Filters.RemoveAt(0);
                    _filter.Dispose();
                }

                if (m_GraphBuilder != null)
                {
                    Marshal.ReleaseComObject(m_GraphBuilder);
                    m_GraphBuilder = null;
                }

                GC.Collect();
                return NOERROR;
            }
            catch
            {
                return E_FAIL;
            }
        }

        #endregion

        #region Protected Methods

        protected virtual HRESULT Load()
        {
            var hr = InitInterfaces();
            if (hr.Succeeded)
                if (OnPlaybackPrepared != null)
                    OnPlaybackPrepared(this, EventArgs.Empty);
            return hr;
        }

        protected virtual HRESULT Unload()
        {
            return CloseInterfaces();
        }

        protected virtual HRESULT OnInitInterfaces()
        {
            return NOERROR;
        }

        protected virtual HRESULT OnCloseInterfaces()
        {
            return NOERROR;
        }

        protected virtual void SettingUpVideoWindow()
        {
            if (m_VideoWindow != null)
            {
                if (m_VideoControl != null)
                {
                    m_VideoWindow.put_Owner(m_VideoControl.Handle);
                    m_VideoWindow.put_MessageDrain(m_VideoControl.Handle);
                    m_VideoWindow.put_WindowStyle(0x40000000 | 0x04000000);
                    ResizeVideoWindow();
                }

                m_VideoWindow.put_AutoShow(Visible ? -1 : 0);
                m_VideoWindow.put_Visible(Visible ? -1 : 0);
            }
        }

        protected virtual HRESULT PreparePlayback()
        {
            m_MediaControl = (IMediaControl)m_GraphBuilder;
            m_BasicVideo = (IBasicVideo)m_GraphBuilder;
            m_MediaSeeking = (IMediaSeeking)m_GraphBuilder;
            m_VideoWindow = (IVideoWindow)m_GraphBuilder;
            m_MediaEventEx = (IMediaEventEx)m_GraphBuilder;
            m_FrameStep = (IVideoFrameStep)m_GraphBuilder;
            SettingUpVideoWindow();
            var hr = m_MediaEventEx.SetNotifyWindow(m_EventControl.Handle, WM_GRAPHNOTIFY,
                Marshal.GetIUnknownForObject(this));
            Debug.Assert(hr == 0);
            SetVolume(m_bMute ? -10000 : m_iVolume);
            if (m_dRate != 1.0)
            {
                m_MediaSeeking.SetRate(m_dRate);
                m_MediaSeeking.GetRate(out m_dRate);
            }

            if (OnPlaybackReady != null) OnPlaybackReady(this, EventArgs.Empty);
            return (HRESULT)hr;
        }

        protected virtual bool ProcessGraphMessage()
        {
            if (m_MediaEventEx != null)
            {
                var hr = 0;
                int _param1, _param2;
                EventCode _code;
                while (hr == 0)
                {
                    hr = m_MediaEventEx.GetEvent(out _code, out _param1, out _param2, 20);
                    if (hr == 0)
                    {
                        hr = m_MediaEventEx.FreeEventParams(_code, _param1, _param2);

                        if (_code == EventCode.Complete) return true;
                        if (_code == EventCode.ErrorAbort) return true;
                        if (_code == EventCode.DeviceLost) return true;
                    }
                }
            }

            return false;
        }

        protected virtual void ResizeVideoWindow()
        {
            if (m_VideoWindow != null && m_VideoControl != null)
                m_VideoWindow.SetWindowPosition(0, 0, m_VideoControl.Width, m_VideoControl.Height);
        }

        #endregion

        #region Overridden Methods

        private void VideoControl_VisibleChanged(object sender, EventArgs e)
        {
            if (m_VideoWindow != null && m_VideoControl != null && m_bShouldPreview)
            {
                m_VideoWindow.put_AutoShow(m_VideoControl.Visible ? -1 : 0);
                m_VideoWindow.put_Visible(m_VideoControl.Visible ? -1 : 0);
            }
        }

        private void VideoControl_Resize(object sender, EventArgs e)
        {
            ResizeVideoWindow();
        }

        #endregion
    }

    [ComVisible(false)]
    public interface ISourceFileSupport
    {
        string FileName { get; set; }
        HRESULT Open();
        HRESULT Open(bool bStart);
        HRESULT Open(string sFileName);
        HRESULT Open(string sFileName, bool bStart);
    }

    [ComVisible(false)]
    public interface IFileDestSupport
    {
        string OutputFileName { get; set; }
        HRESULT Save();
        HRESULT Save(bool bStart);
        HRESULT Save(string sFileName);
        HRESULT Save(string sFileName, bool bStart);
    }

    [ComVisible(false)]
    public class DSFilePlayback : DSFilterGraphBase, ISourceFileSupport
    {
        #region Variables

        protected string m_sFileName = "";

        #endregion

        #region Properties

        public string FileName
        {
            get => m_sFileName;
            set => Open(value);
        }

        #endregion

        #region Overridden Methods

        protected override HRESULT OnInitInterfaces()
        {
            var hr = m_GraphBuilder.RenderFile(m_sFileName, null);
            if (hr < 0) Marshal.ThrowExceptionForHR(hr);
            return (HRESULT)hr;
        }

        #endregion

        #region Public Methods

        public HRESULT Open()
        {
            return Open(true);
        }

        public HRESULT Open(bool bStart)
        {
            return Open(m_sFileName, bStart);
        }

        public HRESULT Open(string sFileName)
        {
            return Open(sFileName, true);
        }

        public virtual HRESULT Open(string sFileName, bool bStart)
        {
            if (sFileName != null && sFileName != "")
            {
                m_sFileName = sFileName;
                if (bStart)
                    return Start();
                return Load();
            }

            return E_POINTER;
        }

        #endregion
    }

    #endregion
}