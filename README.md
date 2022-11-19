# Wissance.MossbauerLab.Watcher

Mossbauer laboratory event watcher and notifier, used for add some specific controls (custom) over Mossbauer spectrometric devices.

### Key features
* `SM2201` spectrometer controlled by software that is running on **`Windows 98`**
* ***Target Platform where `Wissance.MossbauerLab.Watcher` is running - `Raspberry Pi (v2)` + `Raspbian` as OS***

## 1. Watching objects and notifications

### 1.1 Watching objects
* `Messbauer spectra` accumulation process (monitoring auto save process) from `SM2201` spectrometer (controll by date+time)
   of file last change (on `Windows98` machine system ***clock must be set right***)

### 1.2 Notifications 

* Send fresh (-2h of Date,Time) spectra by e-mail and Telegram bot (*NOT IMPLEMENTED YET*)
* Analyze power loss (*NOT IMPLEMENTED YET*)

## 2. Configuration

There are 2 run profiles (`Development` and `Production`) and 2 possible configs variants of `Wissance.Mossabuer.LabWatcher`:

1. Running on Windows allows to use Net Directory class with network path to files && folders, therefore `SM2201` config section
   looks like:
   ```csharp
   "Sm2201SpectraStoreSettings": {
      "Address": "192.168.10.212",
      "Domain": "MOSSBAUERLAB",
      "Folder": "Autosaves",
      "UserCredentials": null
   }
   ```
   
2. Running on any Linux requires to mount Windows shared folder using `cifs.mount`, because `Directory` in Linux doesn't
   understand Windows network Path, therefore `SM2201` config section looks like:
   ```csharp
   "Sm2201SpectraStoreSettings": {
      "Address": "",
      "Domain": "MOSSBAUERLAB",
      "Folder": "/mnt/sm2201/dev",
      "UserCredentials": null
   }
   ```

Solution could be run either on `Windows` or `Linux`
### 2.1 Running solution on Raspberry Pi
1. Configure SMB (/etc/samba/smb.conf) as follows, add to `global` section:
   * `client min protocol = NT1`
   * `client lanman auth = yes'
   * `client ntlmv2 auth = no`
   To check is there access to shared folder using `smbclient` use following command
   ```bash
   smbclient //MICHAEL/Autosaves -m NT1 -w MOSSBAUERLAB
   ```
   Where:
   * `MICHAEL` - computer name
   * `MOSSBAUERLAB` - workgroup name
   * `AUTOSAVES` - name of shared folder
2. To watch save process we monitor windows shared folder but in linux we have to mount shared folder using `cifs`(All names must be Uppercase):
   `sudo mount -t cifs -o user=guest,pass=,vers=1.0,sec=none,domain=MOSSBAUERLAB,ip=192.168.10.217,servern=MICHAEL //MICHAEL/AUTOSAVES /mnt/sm2201/dev`
   Where:
   * `MICHAEL` - computer name
   * `MOSSBAUERLAB` - workgroup name
   * `AUTOSAVES` - name of shared folder
   

## 3. How to run

1. Configure `appsettings.Production.json` before run and set `Address` to "" and set `Folder` to mount point if you are running 
   solution on `Raspberry Pi`
2. Run solution `dotnet Wissance.MossbauerLab.Watcher.Web.dll --environment=Production`