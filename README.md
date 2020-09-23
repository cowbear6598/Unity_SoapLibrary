## UpdateManager

- 使用 UpdateManager.Instance.RequestUpdate / Remove 來將要每偵要更新的 Function 加入
- 目前只包含常用的 Update & FixedUpdate

---

## SoapUtils
### 特效

- SoapUtils.EnableEmission 啟用 / 關閉特效
- SoapUtils.GetEmissionRate 取得特效產生數量
- SoapUtils.SetEmissionRate 設定特效產生數量

### 雜湊

- SoapUtils.EncodeToSha256 將字串雜湊成 Sha256
- SoapUtils.EncodeToHMAC_SHA1 將字串雜湊成 HMACSHA1

### UI

- SoapUtils.SetCanvasGroup 快速設定 Canvas Group 的數值
- SoapUtils.SetColorAlpha 快速設定 Alpha

### 時間

- SoapUtils.ConvertToLocalDateTime
- SoapUtils.ConvertToOrderLocalDate
- SoapUtils.ConvertToOrderLocalDateTime

---

## SoundManager <font color=#FF0000> 測試階段 </font>

> 綁定 Addressable Asset 使用
>
> Soap -> SoundSetting 初始設定是否需要用到 Audio Mixed Group 以及音效數量
>
> 變數由 AduioClip -> AssetReferenceAudioClip 來設定使用的音效


- SoundManager.Instance.Play2D 播放 2D 音效
- SoundManager.Instance.Play3D 播放 3D 音效
- SoundManager.Instance.PlayBGM 更換背景音樂

---

## MysqlManager

> Soap -> Internet -> MysqlSetting 設定需要用到的 domain

- MysqlManager.OnConnetFail 綁定事件，當網路連線出錯時呼叫
- MysqlManager.RunRequestAPIByPost 要求 POST 方法
- MysqlManager.RunRequestAPIByGet 要求 GET 方法

---

## DesignPattern

- SingeletonMonoBehaviour 單例模式，繼承此程式碼並實作 IsNeedDontDestoryOnLoad 決定是否使用

---

## UI 相關

- CanvasResolutionHandle 掛在有 Canvas Scaler 的地方，自適應各種比例去調整 Match Width Or Height

### InfiniteScrollRect <font color=#FF0000> 測試階段 </font>

- SetCellCount 設定 Cell 的格數
- Cell_AddListener 可以綁定當 Cell 改變時的事件
- IInfiniteScrollCellUpdate 當 Cell 更新時會執行 OnInfiniteScrollCellUpdate

---

## 額外使用到的插件

- More Effect Coroutines (Free)
- Json.Net