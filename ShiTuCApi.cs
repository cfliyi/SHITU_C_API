using System;
using System.Runtime.InteropServices;
using System.Text; 
namespace ShiTuApiTestApp
{
    internal static class ShiTuCApi
    {
        // --- DLL Name ---
        private const string DllName = "shitu_c_api.dll";

        // --- Enums (mirroring C enums) ---

        public enum ShiTuApiResultCode
        {
            SHITU_API_SUCCESS = 0,
            SHITU_API_ERROR_GENERAL = -1,
            SHITU_API_ERROR_INVALID_PARAM = -2,
            SHITU_API_ERROR_NOT_INITIALIZED = -3,
            SHITU_API_ERROR_CONFIG_ERROR = -4,
            SHITU_API_ERROR_INDEX_ERROR = -5,
            SHITU_API_ERROR_SEARCH_ERROR = -6,
            SHITU_API_ERROR_FILE_NOT_FOUND = -7,
            SHITU_API_ERROR_MEMORY_ALLOC = -8,
            SHITU_API_ERROR_INTERNAL = -9,
            SHITU_API_ERROR_NOT_IMPLEMENTED = -10,
            SHITU_API_ERROR_GPU_ERROR = -11,
            SHITU_API_ERROR_IMAGE_ERROR = -12,
            SHITU_API_ERROR_LICENSE_ERROR = -13, 
            SHITU_API_ERROR_DEPRECATED = -14   
        }

        public enum ShiTuApiLicenseStatus
        {
            SHITU_API_LICENSE_STATUS_NOT_INITIALIZED = 0,
            SHITU_API_LICENSE_STATUS_VALID_LICENSE = 1,
            SHITU_API_LICENSE_STATUS_INVALID_LICENSE = 2,
            SHITU_API_LICENSE_STATUS_EXPIRED_LICENSE = 3,
            SHITU_API_LICENSE_STATUS_TRIAL_ACTIVE = 4,
            SHITU_API_LICENSE_STATUS_TRIAL_EXPIRED = 5,
            SHITU_API_LICENSE_STATUS_TIME_TAMPERED = 6,
            SHITU_API_LICENSE_STATUS_INTEGRITY_FAILED = 7,
            SHITU_API_LICENSE_STATUS_HARDWARE_ERROR = 8,
            SHITU_API_LICENSE_STATUS_CORRUPTED_TRIAL = 9,
            SHITU_API_LICENSE_STATUS_FIRST_RUN_NEW_TRIAL = 10,
            SHITU_API_LICENSE_STATUS_UNKNOWN_ERROR = 99
        }

        public enum ShiTuApiImageFormat
        {
            SHITU_API_IMAGE_FORMAT_BGR = 0,
            SHITU_API_IMAGE_FORMAT_RGB = 1
        }

        public struct CShiTuLicenseInfo
        {
            public ShiTuApiLicenseStatus status_code;
            public IntPtr status_string;
            public IntPtr error_message;
            public IntPtr machine_code;
            public IntPtr effective_date;
            public IntPtr expiry_date;
            public IntPtr remaining_days;
            public int is_permanent; // 0 for false, 1 for true

            public string GetStatusString() => status_string == IntPtr.Zero ? "" : Marshal.PtrToStringUTF8(status_string);
            public string GetErrorMessage() => error_message == IntPtr.Zero ? "" : Marshal.PtrToStringUTF8(error_message);
            public string GetMachineCode() => machine_code == IntPtr.Zero ? "" : Marshal.PtrToStringUTF8(machine_code);
            public string GetEffectiveDate() => effective_date == IntPtr.Zero ? "" : Marshal.PtrToStringUTF8(effective_date);
            public string GetExpiryDate() => expiry_date == IntPtr.Zero ? "" : Marshal.PtrToStringUTF8(expiry_date);
            public string GetRemainingDays() => remaining_days == IntPtr.Zero ? "" : Marshal.PtrToStringUTF8(remaining_days);
        }

        // --- Structs (mirroring C structs) ---
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)] // Using Ansi temporarily, prefer UTF8 if needed
        public struct CShiTuAPIConfig
        {
            // Using IntPtr for strings to avoid complex marshalling from struct
            public IntPtr det_model_dir; // const char*
            public IntPtr rec_model_dir; // const char*
            public int feature_dim; // int32_t
            public IntPtr index_output_base_path; // const char*

            public int use_gpu;
            public int gpu_id;
            public int gpu_mem;
            public int cpu_threads;
            public int enable_mkldnn;
            public int use_tensorrt;
            public int use_fp16;
            public int feature_norm;

            public int enable_detector;
            public float det_threshold;
            public int det_max_results;
            public IntPtr det_label_list; // const char* const*
            public ulong det_label_list_count; // size_t
            public IntPtr det_input_shape; // const int32_t*
            public ulong det_input_shape_count; // size_t

            public IntPtr index_build_method; // const char*
            public IntPtr index_metric_type; // const char*

            public float search_score_threshold;
            public float search_nms_threshold;

            public int build_list_delimiter; // Use int (char might have marshalling issues)
        }


        [StructLayout(LayoutKind.Sequential)]
        public struct CShiTuBoundingBox
        {
            public int x1; // int32_t
            public int y1; // int32_t
            public int x2; // int32_t
            public int y2; // int32_t

            public override string ToString() => $"[{x1},{y1},{x2},{y2}]";
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct CShiTuSearchResult
        {
            public long id;           // int64_t
            public IntPtr label;      // char* (Needs manual marshalling and freeing via API)
            public float score;       // float
            public float distance;    // float
            public CShiTuBoundingBox box; // Nested struct

            // Helper method to get managed string (caller should be aware IntPtr needs freeing via API)
            public string GetLabelString() => label == IntPtr.Zero ? null : Marshal.PtrToStringUTF8(label);
        }

        // --- Callback Delegate ---
        [UnmanagedFunctionPointer(CallingConvention.Cdecl)]
        public delegate void CShiTuProgressCallback(int current, int total, IntPtr message, IntPtr userData);

        // --- P/Invoke Function Declarations ---

        // Handle Management
        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern ShiTuApiResultCode ShiTuApi_Create(out IntPtr handle_out);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern ShiTuApiResultCode ShiTuApi_Destroy(ref IntPtr handle_ptr); // Use ref for pointer-to-pointer modification

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern int ShiTuApi_IsInitialized(IntPtr handle);

        // Initialization
        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern ShiTuApiResultCode ShiTuApi_InitializeWithLicense(IntPtr handle, [MarshalAs(UnmanagedType.LPUTF8Str)] string yaml_config_path);


        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern ShiTuApiResultCode ShiTuApi_GetLicenseInfo(IntPtr handle, out CShiTuLicenseInfo license_info_out);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void ShiTuApi_FreeLicenseInfoContents(ref CShiTuLicenseInfo license_info);


        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern ShiTuApiResultCode ShiTuApi_ActivateLicenseKey(IntPtr handle, [MarshalAs(UnmanagedType.LPUTF8Str)] string license_key);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern ShiTuApiResultCode ShiTuApi_RegisterLicenseKey(IntPtr handle,
                                                                    [MarshalAs(UnmanagedType.LPUTF8Str)] string license_key);


        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern ShiTuApiResultCode ShiTuApi_GetAllIdsAndLabels(IntPtr handle,
                                                                            out IntPtr ids_out,      
                                                                            out IntPtr labels_out,  
                                                                            out ulong count_out);   

        // 新增：释放GetAllIdsAndLabels返回的内存
        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void ShiTuApi_FreeAllIdsAndLabels(IntPtr ids, IntPtr labels, ulong count);

        // Index Operations
        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern ShiTuApiResultCode ShiTuApi_BuildIndexFromFile(IntPtr handle,
                                                                            [MarshalAs(UnmanagedType.LPUTF8Str)] string image_list_path,
                                                                            CShiTuProgressCallback callback, // Pass delegate instance
                                                                            IntPtr user_data);               // Pass IntPtr.Zero or custom data ptr

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern ShiTuApiResultCode ShiTuApi_SaveIndex(IntPtr handle);

        // Search Operations
        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern ShiTuApiResultCode ShiTuApi_SearchByPath(IntPtr handle,
                                                                      [MarshalAs(UnmanagedType.LPUTF8Str)] string query_image_path,
                                                                      float score_thr_override,
                                                                      float nms_thr_override,
                                                                      int max_results_override,
                                                                      out IntPtr results_out, // Returns pointer to C array
                                                                      out int num_results_out);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern ShiTuApiResultCode ShiTuApi_SearchByData(IntPtr handle,
                                                                      byte[] image_data, // Pass byte array directly
                                                                      int width,
                                                                      int height,
                                                                      int channels,
                                                                      int stride,
                                                                      ShiTuApiImageFormat image_format,
                                                                      float score_thr_override,
                                                                      float nms_thr_override,
                                                                      int max_results_override,
                                                                      out IntPtr results_out,
                                                                      out int num_results_out);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl, EntryPoint = "ShiTuApi_SearchByData")] 
        public static extern ShiTuApiResultCode ShiTuApi_SearchByDataPtr(IntPtr handle,
                                                                         IntPtr image_data, // Pass pointer
                                                                         int width,
                                                                         int height,
                                                                         int channels,
                                                                         int stride,
                                                                         ShiTuApiImageFormat image_format,
                                                                         float score_thr_override,
                                                                         float nms_thr_override,
                                                                         int max_results_override,
                                                                         out IntPtr results_out,
                                                                         out int num_results_out);


        // Deletion Operations
        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern ShiTuApiResultCode ShiTuApi_DeleteItem(IntPtr handle, long id);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern ShiTuApiResultCode ShiTuApi_DeleteItems(IntPtr handle,
                                                                     long[] ids, // Pass array of longs
                                                                     ulong num_ids, // Use ulong for size_t
                                                                     out ulong num_deleted_out); // Use ulong for size_t*

        // Information Retrieval
        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern ShiTuApiResultCode ShiTuApi_GetIndexDimension(IntPtr handle, out int dimension_out);

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern ShiTuApiResultCode ShiTuApi_GetIndexNumItems(IntPtr handle, out ulong num_items_out); // Use ulong for size_t*

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern ShiTuApiResultCode ShiTuApi_GetLabelById(IntPtr handle, long id, out IntPtr label_out); // Returns char**

        // Data Export
        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern ShiTuApiResultCode ShiTuApi_ExportLabels(IntPtr handle, [MarshalAs(UnmanagedType.LPUTF8Str)] string csv_file_path);

        // Memory Management
        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void ShiTuApi_FreeResults(IntPtr results, int num_results); // Takes the original C pointer

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        public static extern void ShiTuApi_FreeString(IntPtr label); // Takes the original C pointer

        [DllImport(DllName, CallingConvention = CallingConvention.Cdecl, ExactSpelling = true)]
        [return: MarshalAs(UnmanagedType.LPUTF8Str)] 
        public static extern string ShiTuApi_GetLastErrorMsg();
    }
}