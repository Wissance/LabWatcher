# Wissance.MossbauerLab.Watcher
A set of tools that helps to analyze **state of spectrometry devices** and notify about important events in **scientific lab**.

![GitHub code size in bytes](https://img.shields.io/github/languages/code-size/wissance/LabWatcher?style=plastic) 
![GitHub issues](https://img.shields.io/github/issues/wissance/LabWatcher?style=plastic)
![GitHub Release Date](https://img.shields.io/github/release-date/wissance/LabWatcher) 
![GitHub release (latest by date)](https://img.shields.io/github/downloads/wissance/LabWatcher/v1.0/total?style=plastic)

![LabWatcher: is automated Mossbauer laboratory control toolset](/docs/img/labwatcher.jpg)

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
   * `client lanman auth = yes`
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

### 3.1 Run once
1. Configure `appsettings.Production.json` before run and set `Address` to "" and set `Folder` to mount point if you are running 
   solution on `Raspberry Pi`
2. Run solution `dotnet Wissance.MossbauerLab.Watcher.Web.dll --environment=Production`

### 3.2 Run as a service

To run `Wissance.MossbauerLab.Watcher.Web` as a service it should be configured as `systemd` service:

1. Create group - `sudo groupadd mossbauer`
2. Create user - `labwatcher - sudo useradd -r -g mossbauer -d /usr/local/sbin/labwatcher -s /sbin/nologin labwatcher`
3. Create labwatcher.service file with following content:
```

[Unit]
Description=Wissance.MossbauerLab.Watcher service
After=syslog.target network.target
Before=httpd.service
[Service]
WorkingDirectory=/usr/local/sbin/labwatcher/app
User=labwatcher
Group=mossbauer
LimitNOFILE=102642
PIDFile=/var/run/mossbauer/labwatcher.pid
ExecStart=dotnet /usr/local/sbin/labwatcher/app/Wissance.MossbauerLab.Watcher.Web.dll --environment=Production
StandardOutput=syslog
StandardError=syslog

[Install]
WantedBy=multi-user.target

```
4. Change directory owner to `mossbauer:labwatcher` - `sudo chown -R labwatcher:mossbauer /usr/local/sbin/labwatcher`
5. Copy `labwatcher.service` file to `/etc/systemd/system`
6. Execute `sudo systemctl daemon-reload`
7. Execute `sudo systemctl enable labwatcher`
8. Execute `sudo systemctl start labwatcher`

### 4. Troubleshooting

1. If we are mount automatically and got error like:
```
Jan 08 18:14:32 LabControl mount.sh[495]: mount error(101): Network is unreachable
Jan 08 18:14:32 LabControl mount.sh[495]: Refer to the mount.cifs(8) manual page (e.g. man mount.cifs) ...
```
there are many solutions but for me helpful was adding `sleep 20s` in `.sh` script prior to `mount`
