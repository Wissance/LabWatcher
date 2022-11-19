# LabWatcher
Mossbauer laboratory watcher and notifier

## Watching objects

* `Messbauer spectra` accumulation process (monitoring auto save process) from `SM2201` spectrometer

## Running solution on Raspberry Pie
1. Configure SMB (/etc/samba/smb.conf) as follows, add to `global` section:
   * `client min protocol = NT1`
   * `client lanman auth = yes'
   * `client ntlmv2 auth = no`
2. To watch save process we monitor windows shared folder but in linux we have to mount shared folder using `cifs`(All names must be Uppercase):
   `sudo mount -t cifs -o user=guest,pass=,vers=1.0,sec=none,domain=MOSSBAUERLAB,ip=192.168.10.217,servern=MICHAEL //MICHAEL/AUTOSAVES /mnt/sm2201/dev`
   Where:
   * `MICHAEL` - computer name
   * `MOSSBAUERLAB` - workgroup name
   * `AUTOSAVES` - name of shared folder