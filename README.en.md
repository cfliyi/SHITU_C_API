🌐 [简体中文](README.cn.md) | English

# **ShiTu C API User Manual**

**Version:** 1.0
**Last Updated:** 2024-05-23

## **1. Introduction**

The ShiTu C API provides a comprehensive C-language interface for accessing underlying functionalities of image recognition, feature extraction, object detection, and vector indexing. This API is meticulously engineered by refactoring Baidu PaddlePaddle's **PaddleClas** vision toolkit, aiming to offer developers a stable, user-friendly, and high-performance solution for AI vision tasks.

By hiding complex internal dependencies such as **Paddle Inference v3.0.0-beta1**, **OpenCV 4.11.0**, and **FAISS 1.7.4**, ShiTuApi delivers a consistent and reliable interface that can be easily integrated into various programming languages, including C/C++, C#, and Python.

**Core Features:**
*(Verified against the `ShiTuApiTestApp` implementation)*
*   **License Management**: Supports a trial mode and can be activated with a license key for commercial use.
*   **Model Loading & Initialization**: Flexibly load and configure detection and recognition models, along with all related parameters, via a single `config.yaml` file.
*   **Index Building**: Supports batch index construction from an image list file (`image_list.txt`).
*   **Image Retrieval**:
    *   Supports efficient similar image/object retrieval by **image file path**.
    *   Supports retrieval from **in-memory image data**, ideal for video streams and camera feeds.
    *   Supports **batch folder searching** with automatic generation of result reports.
*   **Index Management**: Supports **saving** the index and **deleting single or multiple** entries.
*   **Information Query**: Get real-time information about the index, such as its dimension and total vector count, and query labels by their corresponding IDs.
*   **Data Export**: Export the entire index's ID-to-label mappings to a CSV file for data analysis and migration.

## **2. System Requirements and Dependencies**

### **2.1 Runtime Environment (Windows)**

The current version of this API is optimized for the Windows (x64) platform, designed to provide a "green" and lightweight deployment experience.

*   **Operating System**: 64-bit Windows 10 or newer.
*   **C++ Runtime Library**: **Microsoft Visual C++ Redistributable for Visual Studio 2022**. This is a required system component for running DLLs compiled with MSVC. Most end-user systems already have this installed; if not, your application's installer should include it.

> **Cross-Platform Support**: We offer professional services to provide customized builds for **Linux**, **Embedded Linux**, or other operating systems based on your commercial needs. Please contact us for more information.

### **2.2 Development Environment Dependencies (Internal Reference)**

The compilation and building of this API rely on the following core libraries:

*   **C++ Compiler**: Microsoft Visual C++ (MSVC) v143 or newer (Visual Studio 2022).
*   **Core Libraries**:
    *   Inference Engine: **Paddle Inference v3.0.0-beta1**
    *   Image Processing: **OpenCV 4.11.0**
    *   Vector Search: **FAISS 1.7.4**
    *   Configuration Parsing: **yaml-cpp**

## **3. API Files**

To integrate the ShiTu C API into your project, you will need the following files:

*   **`shitu_c_api.h`**: **Required**. The C API header file, which contains all function declarations, structure definitions, and enumerations.
*   **`shitu_c_api.dll`**: **Required**. The compiled 64-bit dynamic-link library. Your application needs this file at runtime.
*   **`shitu_c_api.lib`**: **Required (for C++)**. The import library needed when linking against the DLL in an MSVC-compiled C/C++ project.

## **4. API Usage Workflow**

A typical API usage workflow is as follows, which is fully demonstrated in the `ShiTuApiTestApp` sample application:

1.  **Create a Handle**: Call `ShiTuApi_Create()` to obtain an API instance handle (`ShiTuApiHandle`). This is the first step for all operations.
2.  **Initialize the API**: Call `ShiTuApi_InitializeWithLicense()` and pass the path to your `config.yaml` file. This is the **only recommended initialization method**. It automatically handles license status checks and initializes all modules.
3.  **Check Initialization Status**: Call `ShiTuApi_IsInitialized()` to confirm that the API is ready for use.
4.  **Perform Operations**: Call other API functions as needed, for example:
    *   `ShiTuApi_BuildIndexFromFile()`: Build or append to the index from an `image_list.txt`.
    *   `ShiTuApi_SearchByPath()`: Perform recognition on a single image.
    *   `ShiTuApi_DeleteItem()` / `ShiTuApi_DeleteItems()`: Remove entries from the index.
    *   `ShiTuApi_GetLabelById()`: Query a label by its ID.
    *   `ShiTuApi_SaveIndex()`: Persist changes in the in-memory index to disk.
5.  **Handle Results and Memory**:
    *   **The caller is responsible for freeing memory allocated by the API** to prevent memory leaks.
    *   Use `ShiTuApi_FreeResults()` to free the result array returned by `ShiTuApi_Search*` functions.
    *   Use `ShiTuApi_FreeString()` to free the string returned by `ShiTuApi_GetLabelById`.
    *   Use `ShiTuApi_FreeLicenseInfoContents()` to free the strings within the license information structure.
6.  **Error Handling**: After each API call, check the returned `ShiTuApiResultCode`. If it is not `SHITU_API_SUCCESS`, call `ShiTuApi_GetLastErrorMsg()` to get a detailed error message string for debugging.
7.  **Destroy the Handle**: When the API instance is no longer needed, you **must** call `ShiTuApi_Destroy()` to release all associated resources.

## **5. Core Data Structures**

*   **`ShiTuApiHandle`**: An opaque pointer representing an API instance.
*   **`CShiTuLicenseInfo`**: A structure containing license status, machine code, expiration date, etc.
*   **`CShiTuSearchResult`**: A structure containing information for a single search result:
    *   `id` (`int64_t`): The ID of the matched entry in the index.
    *   `label` (`char*`): The label string of the match (UTF-8 encoded).
    *   `score` (`float`): The recognition/match score (typically 0.0 to 1.0).
    *   `distance` (`float`): The feature vector distance (L2 or IP).
    *   `box` (`CShiTuBoundingBox`): The coordinates of the detected bounding box.

## **6. Function Reference**

(This list is verified against the current codebase. For detailed parameter descriptions, please refer to the comments in `shitu_c_api.h`.)

*   **Handle Management**: `ShiTuApi_Create`, `ShiTuApi_Destroy`, `ShiTuApi_IsInitialized`
*   **Initialization**: `ShiTuApi_InitializeWithLicense`
*   **License**: `ShiTuApi_ActivateLicenseKey`, `ShiTuApi_GetLicenseInfo`, `ShiTuApi_FreeLicenseInfoContents`
*   **Index Operations**: `ShiTuApi_BuildIndexFromFile`, `ShiTuApi_SaveIndex`, `ShiTuApi_DeleteItem`, `ShiTuApi_DeleteItems`
*   **Retrieval Operations**: `ShiTuApi_SearchByPath`, `ShiTuApi_SearchByData`
*   **Information Query**: `ShiTuApi_GetIndexDimension`, `ShiTuApi_GetIndexNumItems`, `ShiTuApi_GetLabelById`
*   **Data Export**: `ShiTuApi_ExportLabels`, `ShiTuApi_GetAllIdsAndLabels`, `ShiTuApi_FreeAllIdsAndLabels`
*   **Memory Management**: `ShiTuApi_FreeResults`, `ShiTuApi_FreeString`
*   **Error Retrieval**: `ShiTuApi_GetLastErrorMsg`

## **7. Configuration File (`config.yaml`) Explained**

The API's behavior is primarily controlled by the `config.yaml` file. **All path settings support both relative paths (relative to the DLL's runtime directory) and absolute paths.**

*   **`Global` Section:**
    *   `enable_mkldnn`: Set to `true` to enable Intel MKL-DNN acceleration in CPU mode.
    *   `det_inference_model_dir` & `rec_inference_model_dir`: Paths to the **detection model** and **recognition model**. The recognition model is required; the detection model is optional.
    *   `feature_dim`: The feature vector dimension (**required**). Must match the output dimension of your recognition model.
    *   `threshold`: The **detection confidence threshold**. Filters out low-confidence detection boxes.
    *   `visualize_...` & `freetype_font_path`: Control the generation of visualized result images and specify the font for displaying non-ASCII characters like Chinese.

*   **`IndexProcess` Section:**
    *   `index_dir`: The base path for your index files (**required**).
    *   `index_method`: The FAISS index factory string. `"IDMap,Flat"` is the most versatile method for exact search and supports deletion.
    *   `metric_type`: The distance metric ("L2" or "IP"). "IP" is recommended for normalized features.
    *   `threshold`: The **recognition score threshold**. This is the "passing score" for a match to be considered a positive identification.
    *   `use_detBuild`: **Index building mode**. `false` for whole-image indexing (good for logos, icons); `true` for detect-then-index (better for products in complex scenes).

## **8. Error Handling**

*   **Return Values**: All core API functions return a `ShiTuApiResultCode`. Always check if the return value is `SHITU_API_SUCCESS`.
*   **Error Messages**: If a function fails, immediately call `ShiTuApi_GetLastErrorMsg()` to get a detailed, human-readable error description (UTF-8 encoded).
*   **Log File**: The API may generate a `shitu_api_log.txt` file in the application's directory. This file contains more detailed internal debug information that can be useful for troubleshooting complex issues.

## **9. Thread Safety**

*   **Handle Instances**: **A single `ShiTuApiHandle` instance is thread-safe**. You can create one global handle and call its functions (e.g., `ShiTuApi_SearchByPath`) from multiple worker threads simultaneously. An internal mutex will ensure atomic operations and data integrity.
*   **Callbacks**: The progress callback for `ShiTuApi_BuildIndexFromFile` is executed on an internal worker thread. **Do not perform long-running operations or access UI controls directly from the callback**. If you need to update the UI, you must use your framework's thread-safe invocation mechanism (e.g., `Invoke`/`BeginInvoke` in C#).

## **10. Code Example (`ShiTuApiTestApp` - C#)**

We provide a comprehensive C# WinForms test application, which serves as the best starting point for learning how to use the API.

**Core Invocation Logic (Simplified):**
```csharp
// Use the namespace for the API wrappers
using static ShiTuApiTestApp.ShiTuCApi;

// ...

IntPtr apiHandle = IntPtr.Zero;

try 
{
    // 1. Create the handle
    if (ShiTuApi_Create(out apiHandle) != ShiTuApiResultCode.SHITU_API_SUCCESS) 
        throw new Exception("Failed to create handle");

    // 2. Initialize the API (includes license check)
    if (ShiTuApi_InitializeWithLicense(apiHandle, "config.yaml") != ShiTuApiResultCode.SHITU_API_SUCCESS)
        throw new Exception($"Initialization failed: {ShiTuApi_GetLastErrorMsg()}");

    // 3. Perform a search
    IntPtr resultsPtr = IntPtr.Zero;
    int numResults = 0;
    var searchResult = ShiTuApi_SearchByPath(apiHandle, "path/to/your/image.jpg", -1f, -1f, -1, out resultsPtr, out numResults);
    
    if (searchResult == ShiTuApiResultCode.SHITU_API_SUCCESS && numResults > 0)
    {
        Console.WriteLine($"Successfully found {numResults} results.");
        // ... Parse resultsPtr here ...
    }
    else 
    {
        Console.WriteLine($"Search failed: {ShiTuApi_GetLastErrorMsg()}");
    }
}
catch (Exception ex)
{
    Console.WriteLine($"An error occurred: {ex.Message}");
}
finally
{
    // 4. Clean up resources
    // if (resultsPtr != IntPtr.Zero) ShiTuApi_FreeResults(resultsPtr, numResults); // Free search results
    if (apiHandle != IntPtr.Zero) ShiTuApi_Destroy(ref apiHandle); // Destroy the handle
}
```
> **Full Project**: Please refer to the `ShiTuApiTestApp` solution for complete examples of all function calls, UI interaction, background tasks, and best practices for cross-language memory management.

## **11. Important Notes**

*   **Memory Management**: **This is the most critical aspect of using the C API**. The caller **must** be responsible for freeing memory allocated by the API (returned via pointers like `CShiTuSearchResult**` and `char**`) by using the corresponding `ShiTuApi_Free*` functions. Failure to do so will result in serious memory leaks.
*   **Handle Lifecycle**: Always call `ShiTuApi_Destroy` when you are finished with an API instance to release its resources.
*   **Configuration File**: The API's behavior is highly dependent on `config.yaml`. If initialization fails, first check that the file path is correct, the YAML syntax is valid, and all parameters (especially model paths, dimensions, and preprocessing steps) match your models and data.
*   **UTF-8 Encoding**: All string interactions (paths, labels) in the API use UTF-8 encoding. Ensure your calling code and environment correctly handle UTF-8 strings to avoid garbled text issues.
*   **Build Configuration**: Ensure that your application and the `shitu_c_api.dll` are both compiled for the same platform architecture (**x64**) and configuration (**Release**) to avoid compatibility issues.