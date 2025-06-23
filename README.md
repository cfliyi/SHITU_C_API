🌐 简体中文 | [English](README.en.md)

* # **ShiTu C API 使用说明书**

  **版本:** 1.0
  **最后更新日期:** 2025-06-21

  ## **1. 简介**

  ShiTu C API 提供了一套功能强大且高度优化的 C 语言接口，封装了底层的图像识别、特征提取、目标检测和向量索引功能。该API基于百度飞桨**PaddleClas**视觉套件进行深度重构，旨在为开发者提供一个稳定、易用且高性能的AI视觉解决方案。

  通过隐藏内部复杂的依赖（如**Paddle Inference v3.0.0-beta1**、**OpenCV 4.11.0**、**FAISS 1.7.4**），ShiTuApi为开发者提供了一致、可靠的接口，支持在C/C++、C#、Python等多种语言中进行快速集成。

  **核心功能:**
  *(已根据ShiTuApiTestApp代码核实)*

  *   **许可证管理**: 支持试用模式，并可通过许可证密钥激活，保障商业应用。
  *   **模型加载与初始化**: 通过一个独立的 `config.yaml` 文件，灵活加载和配置检测、识别模型及所有相关参数。
  *   **索引构建**: 支持从图像列表文件（`image_list.txt`）批量构建高维特征向量索引。
  *   **图像检索**:
      *   支持通过**图像文件路径**进行高效的相似图像/目标检索。
      *   支持通过**内存中的图像数据**进行检索，适用于视频流、摄像头等场景。
      *   支持**批量文件夹搜索**，并自动生成结果报告。
  *   **索引管理**: 支持向索引中**保存**、**删除单个或批量**的条目。
  *   **信息查询**: 实时获取索引的维度、向量总数，并可通过ID查询对应的标签。
  *   **数据导出**: 支持将整个索引库的ID-标签映射关系导出为CSV文件，便于数据分析和迁移。

  ## **2. 环境要求与依赖**

  ### **2.1 运行时环境 (Windows)**

  本API当前版本专注于Windows (x64) 平台，旨在提供一个“绿色”、轻量级的部署体验。

  *   **操作系统**: 64位的 Windows 10 或更高版本。
  *   **C++ 运行时库**: **Microsoft Visual C++ Redistributable for Visual Studio 2022**。这是运行由MSVC编译器生成的DLL所必需的系统组件。通常，最终用户的系统已安装此组件；如未安装，您的应用程序安装包应包含它。

  > **跨平台支持**: 我们有能力根据您的商业需求，提供在 **Linux**、**嵌入式Linux** 或其他操作系统上运行的定制版本。请联系我们获取更多信息。

  ### **2.2 开发环境依赖 (内部参考)**

  本API的编译和构建依赖于以下核心库：

  *   **C++ 编译器**: Microsoft Visual C++ (MSVC) v143 或更高版本 (Visual Studio 2022)。
  *   **核心库**:
      *   推理引擎: **Paddle Inference v3.0.0-beta1**
      *   图像处理: **OpenCV 4.11.0**
      *   向量检索: **FAISS 1.7.4**
      *   配置解析: **yaml-cpp**

  ## **3. 接口文件**

  要将ShiTu C API集成到您的项目中，您需要以下文件：

  *   **`shitu_c_api.h`**: **必需**。C API头文件，包含了所有函数声明、结构体定义和枚举。
  *   **`shitu_c_api.dll`**: **必需**。编译好的64位动态链接库文件，您的应用程序在运行时需要加载它。
  *   **`shitu_c_api.lib`**: **必需 (对于C++)**。当您使用MSVC编译C/C++代码时，需要链接此导入库。

  ## **4. API 使用流程**

  典型的API使用流程如下，`ShiTuApiTestApp` 示例程序完整地演示了这一流程：

  1.  **创建句柄**: 调用 `ShiTuApi_Create()` 获取一个API实例句柄 (`ShiTuApiHandle`)。这是所有操作的第一步。
  2.  **初始化API**: 调用 `ShiTuApi_InitializeWithLicense()` 并传入 `config.yaml` 文件的路径。这是**唯一推荐的初始化方式**。此函数会自动处理许可证状态检查和所有模块的初始化。
  3.  **检查初始化状态**: 调用 `ShiTuApi_IsInitialized()` 确认API是否已准备就绪。
  4.  **执行操作**: 根据业务需求，调用相应的API函数：
      *   `ShiTuApi_BuildIndexFromFile()`: 从`image_list.txt`构建或追加索引。
      *   `ShiTuApi_SearchByPath()`: 对单张图片进行识别。
      *   `ShiTuApi_DeleteItem()` / `ShiTuApi_DeleteItems()`: 从索引库中移除条目。
      *   `ShiTuApi_GetLabelById()`: 通过ID查询其标签。
      *   `ShiTuApi_SaveIndex()`: 将内存中的索引变更持久化到磁盘。
  5.  **处理结果与内存**:
      *   **调用者必须负责释放由API分配的内存**，以避免内存泄漏。
      *   使用 `ShiTuApi_FreeResults()` 释放由 `ShiTuApi_Search*` 函数返回的结果数组。
      *   使用 `ShiTuApi_FreeString()` 释放由 `ShiTuApi_GetLabelById` 函数返回的字符串。
      *   使用 `ShiTuApi_FreeLicenseInfoContents()` 释放许可证信息结构体中的字符串。
  6.  **错误处理**: 每次调用API函数后，检查返回的 `ShiTuApiResultCode`。如果不是 `SHITU_API_SUCCESS`，则应调用 `ShiTuApi_GetLastErrorMsg()` 获取详细的错误信息字符串用于调试。
  7.  **销毁句柄**: 当应用程序退出或不再需要API实例时，**必须**调用 `ShiTuApi_Destroy()` 来释放所有相关资源。

  ## **5. 核心数据结构**

  *   **`ShiTuApiHandle`**: 代表一个API实例的不透明指针。
  *   **`CShiTuLicenseInfo`**: 包含许可证状态、机器码、有效期等信息的结构体。
  *   **`CShiTuSearchResult`**: 包含单个搜索结果信息的结构体：
      *   `id` (`int64_t`): 匹配到的索引ID。
      *   `label` (`char*`): 匹配到的标签字符串 (UTF-8)。
      *   `score` (`float`): 识别/匹配得分 (0.0 ~ 1.0)。
      *   `distance` (`float`): 特征向量距离 (L2 或 IP)。
      *   `box` (`CShiTuBoundingBox`): 检测到的边界框坐标。

  ## **6. 函数详解**

  (以下函数列表已根据现有代码核实，详细参数说明请参考 `shitu_c_api.h` 文件)

  *   **句柄管理**: `ShiTuApi_Create`, `ShiTuApi_Destroy`, `ShiTuApi_IsInitialized`
  *   **初始化**: `ShiTuApi_InitializeWithLicense`
  *   **许可证**: `ShiTuApi_ActivateLicenseKey`, `ShiTuApi_GetLicenseInfo`, `ShiTuApi_FreeLicenseInfoContents`
  *   **索引操作**: `ShiTuApi_BuildIndexFromFile`, `ShiTuApi_SaveIndex`, `ShiTuApi_DeleteItem`, `ShiTuApi_DeleteItems`
  *   **检索操作**: `ShiTuApi_SearchByPath`, `ShiTuApi_SearchByData`
  *   **信息查询**: `ShiTuApi_GetIndexDimension`, `ShiTuApi_GetIndexNumItems`, `ShiTuApi_GetLabelById`
  *   **数据导出**: `ShiTuApi_ExportLabels`, `ShiTuApi_GetAllIdsAndLabels`, `ShiTuApi_FreeAllIdsAndLabels`
  *   **内存管理**: `ShiTuApi_FreeResults`, `ShiTuApi_FreeString`
  *   **错误获取**: `ShiTuApi_GetLastErrorMsg`

  ## **7. 配置文件 (`config.yaml`) 说明**

  API的行为主要通过 `config.yaml` 文件进行控制。**所有路径配置都支持相对路径（相对于DLL运行目录）和绝对路径**。

  *   **`Global` 区块:**
      *   `enable_mkldnn`: 在CPU模式下，设置为`true`以启用Intel MKL-DNN加速。
      *   `det_inference_model_dir`: **检测模型**路径。设置此路径以启用“先检测后识别”的模式。
      *   `rec_inference_model_dir`: **识别模型**路径 (**必需**)。
      *   `feature_dim`: 特征维度 (**必需**)，必须与识别模型匹配。
      *   `threshold`: **检测置信度阈值**。过滤掉低可信度的检测框。
      *   `visualize_...` & `freetype_font_path`: 控制是否生成带标注的结果图，以及显示中文所需的字体。

  *   **`IndexProcess` 区块:**
      *   `index_dir`: 索引文件的基础路径 (**必需**)。
      *   `index_method`: FAISS索引构建方法。`"IDMap,Flat"` 是最通用、支持删除的精确搜索方法。
      *   `metric_type`: 距离度量 ("L2" 或 "IP")。
      *   `threshold`: **识别得分阈值**。这是识别结果的“录取线”，只有相似度得分高于此值的匹配才会被返回。
      *   `use_detBuild`: **建库模式**。`false`为整图建库, `true`为检测后建库。

  ## **8. 错误处理**

  *   **返回值**: 所有核心API函数都返回 `ShiTuApiResultCode`。请务必检查返回值是否为 `SHITU_API_SUCCESS`。
  *   **错误消息**: 如果函数调用失败，请立即调用 `ShiTuApi_GetLastErrorMsg()` 获取详细的、人类可读的错误描述字符串。
  *   **日志**: API在运行时，可能会在程序目录下生成一个 `shitu_api_log.txt` 文件，其中包含更详细的内部调试信息，有助于定位复杂问题。

  ## **9. 线程安全**

  *   **句柄实例**: **同一个`ShiTuApiHandle`实例是线程安全的**。您可以创建一个全局句柄，然后在多个工作线程中同时调用 `ShiTuApi_SearchByPath` 等函数，API内部的互斥锁会确保操作的原子性和数据的完整性。
  *   **回调函数**: `ShiTuApi_BuildIndexFromFile` 的进度回调函数是在一个内部工作线程中被调用的。**严禁在回调中执行任何耗时操作或直接更新UI控件**。如需更新UI，必须使用UI框架提供的跨线程调用机制（例如C#的 `Invoke`/`BeginInvoke`）。

  ## **10. 示例代码 (`ShiTuApiTestApp` - C#)**

  我们提供了一个功能完备的C# WinForms测试程序，它是学习和使用本API的最佳起点。

  示例程序、模型及动态库文件链接:  
  1、https://pan.baidu.com/s/1JAtsiL5Zh6RSxRrxcp9qAw?pwd=5574 提取码: 5574
  2、https://secure.ue.internxt.com/d/sh/file/9e59d447-e1b5-4e5d-87fb-c3ccd8dca37b/fb906ac390b384e4fe90c84ee14e06f7deaf33c0e84e80507d03cd7b95846716
  3、https://u.pcloud.link/publink/show?code=XZH9Mu5ZwUlEasu9TcpaaAD3cp0CwmObVjoX

  **核心调用逻辑 (简化版):**

  ```csharp
  // 命名空间引用
  using static ShiTuApiTestApp.ShiTuCApi;
  
  // ...
  
  IntPtr apiHandle = IntPtr.Zero;
  
  try 
  {
      // 1. 创建句柄
      if (ShiTuApi_Create(out apiHandle) != ShiTuApiResultCode.SHITU_API_SUCCESS) 
          throw new Exception("创建句柄失败");
  
      // 2. 初始化API (包含许可证检查)
      if (ShiTuApi_InitializeWithLicense(apiHandle, "config.yaml") != ShiTuApiResultCode.SHITU_API_SUCCESS)
          throw new Exception($"初始化失败: {ShiTuApi_GetLastErrorMsg()}");
  
      // 3. 执行搜索
      IntPtr resultsPtr = IntPtr.Zero;
      int numResults = 0;
      var searchResult = ShiTuApi_SearchByPath(apiHandle, "path/to/your/image.jpg", -1f, -1f, -1, out resultsPtr, out numResults);
      
      if (searchResult == ShiTuApiResultCode.SHITU_API_SUCCESS && numResults > 0)
      {
          Console.WriteLine($"成功找到 {numResults} 个结果。");
          // ... 此处解析 resultsPtr ...
      }
      else 
      {
          Console.WriteLine($"搜索失败: {ShiTuApi_GetLastErrorMsg()}");
      }
  }
  catch (Exception ex)
  {
      Console.WriteLine($"发生错误: {ex.Message}");
  }
  finally
  {
      // 4. 清理资源
      // if (resultsPtr != IntPtr.Zero) ShiTuApi_FreeResults(resultsPtr, numResults); // 释放搜索结果
      if (apiHandle != IntPtr.Zero) ShiTuApi_Destroy(ref apiHandle); // 销毁句柄
  }
  ```

  > **完整项目**: 请参考 `ShiTuApiTestApp` 解决方案，它包含了所有函数的调用示例、UI交互、后台任务以及跨语言内存管理的最佳实践。

  ## **11. 注意事项**

  *   **内存管理**: **这是C API使用的重中之重**。任何由API返回并由您接管的指针（如搜索结果、标签字符串），都必须在使用完毕后调用对应的 `ShiTuApi_Free*` 函数来释放，否则将导致严重的内存泄漏。
  *   **配置文件**: API的行为高度依赖 `config.yaml`。在初始化失败时，请首先仔细检查文件路径是否正确、YAML语法是否有误、以及各参数是否与您的模型和数据完全匹配。
  *   **UTF-8编码**: API的所有字符串交互（路径、标签）都默认使用UTF-8编码。请确保您的调用端代码能够正确处理UTF-8字符串，以避免乱码问题。
  *   **编译版本**: 请确保您的应用程序和 `shitu_c_api.dll` 都是用相同的平台架构（**x64**）和配置（**Release**）编译的，以避免兼容性问题。
 
  邮箱地址：cfliyi@outlook.com

  [![Hits](https://hits.sh/github.com/cfliyi/SHITU_C_API.svg)](https://hits.sh/github.com/cfliyi/SHITU_C_API/)
