# 更新資訊

## [1.1.11] - 2020-11-06
### 修改

- 增加中文字體和使用的字
- 增加日文字體和使用的字

## [1.1.10] - 2020-11-06
### 新增

- Fonts 增加日文字

## [1.1.9] - 2020-11-02
### 修改

- CanvasResolutionHandle 公開設定 Scaler 功能
- MysqlManager 修正沒有執行到 multipart/form-data api 的問題
- InfiniteScrollRect 拆成 Verticle 以及 Horizontal ，開始後續擴充
- AddressableAssets 更新至 1.16.7

## [1.1.8] - 2020-10-21
### 修改

- Downlaod 增加公開 Function

## [1.1.7] - 2020-10-21
### 增加

- MysqlManager 增加 multipart/form-data 使用
- DownloadManager 加入，目前測試中

## [1.1.6] - 2020-10-14
### 修改

- MysqlManager 消除 _secure 參數
- MysqlManagerEditorWindow 更改預設網址、選單選項

## [1.1.5] - 2020-10-08
### 新增

- 更新 Addressable 版本

## [1.1.4] - 2020-09-23
### 新增

- README 文件新增簡略說明
- InfiniteScrollRect 進入測試階段

## [1.1.3] - 2020-09-18
### 修改

- SoundManager 修改釋放的方法

## [1.1.2] - 2020-09-17
### 新增

- SoundManager 進入測試階段
- SingeletonMonoBehaviour 修正結束 Play Mode 應該要出現警告而非錯誤

## [1.1.1] - 2020-09-14
### 新增

- SoundManager 工作中，請先不要使用
- SingeletonMonoBehaviour 獨體設計模式加入
- 增加常用的 UI 圖片

## [1.1.0] - 2020-08-18
### 修改

- MysqlManager 的網域名稱改為 List 設定
- MysqlManager 增加 _domainIndex 函式參數 

## [1.0.9] - 2020-08-13
### 修改

- MysqlManager 修正依然無法儲存的 Bug
- MysqlManager 刪除 Log Domain

## [1.0.8] - 2020-08-13
### 增加

- UpdateManager 加入測試

### 修改

- MysqlManager 的 Editor 增加儲存功能，不會再回到 127.0.0.1 預設網域名

## [1.0.7] - 2020-08-12
### 增加

- MysqlManager 擴充為另外的 editor 介面設定，不需要透過修改程式碼才能設定了

## [1.0.6] - 2020-08-11
### 修改

- 常用文字列表增加 "空白"
- MysqlManager Callback 使用 class 方式回傳

## [1.0.5] - 2020-08-10
### 增加

- 增加常用的文字列表

## [1.0.4] - 2020-07-27
### 增加

- 增加一些常用到的 Shader

### 修改

- License 改成 MIT

## [1.0.3] - 2020-07-24
### 增加

- SoapUtils 增加 DateTime 轉換功能

## [1.0.2] - 2020-07-24
### 增加

- SoapUtils 增加雜湊成 Sha1 的功能

## [1.0.1] - 2020-07-24
### 增加

- Json.net、MEC 一些程式碼用到這些免費插件
- MysqlManager.cs 主要用於資料庫之間的傳遞

### 修改

- SoapUtils.cs 移動至 Common 資料夾

## [1.0.0] - 2020-07-24
### 增加

- CanvasResolutionHandle.cs 根據寬跟高調整 UI 對應大小
- SoapUtils.cs 常用的一些功能將會在這新增