# AutoUpdater
A little program to auto update your program.  
![1](https://i.imgur.com/ELBLBeq.png)  
## 使用方式：
將AutoUpdater.exe、AutoUpdater.ini與你的程式放置於同一目錄下  
  
需要檢查更新時執行AutoUpdater.exe
可參考此程式碼：
```C#
    void Update()  
    {  
        ProcessStartInfo startInfo = new ProcessStartInfo("AutoUpdater.exe");  
        startInfo.WindowStyle = ProcessWindowStyle.Minimized;  
        startInfo.Arguments = "-CheckUpdateWithForm";  
        Process.Start(startInfo);  
    }  
```
有三種啟動參數：  
-updateself"              檢查AutoUpdater是否有更新  
-CheckUpdateWithForm      按照AutoUpdater.ini的設定更新，會顯示Form  
-CheckUpdateWithoutForm   按照AutoUpdater.ini的設定更新，不會顯示Form  
  
只支援ZIP的壓縮格式，其他的不支援(7z、rar...)  

### 配置文件：  

#### <AutoUpdater.ini>  
這個檔案與AutoUpdater.exe放於同一目錄下
```ini
    [AutoUpdater_URL]  
    APP_Info_ini=https://sites.google.com/site/pow7000web/mysoftware/autoupdater/範例程式.ini  
      
    [APP_Info]  
    APP_Name=範例程式  
```
***網址那裏必須是一個可以下載的ini檔***  


#### <範例程式.ini>
[請參考這個檔案](https://sites.google.com/site/pow7000web/mysoftware/autoupdater/範例程式.ini)  
可以用記事本打開，建議用Notepad++開啟  
如果List沒有特別指定，則會全部進行覆蓋  
  
`只支援ZIP的壓縮格式，其他的不支援(7z、rar...)`  
  
  
