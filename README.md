Creates a Youtube ASH file from the command line.

### Usage

`yashgen video_id destination [-6]`

`video_id`: The ID of the YouTube video.  
`destination`: The destination folder.  
`-6`: Force IPv6 (optional).

### Requirements
#### Linux
The following libraries / programs are needed:

* libASMedia.so *(from Audiosurf 2/Audiosurf2_Data/Plugins/x86_64/)*  
* libbass.so *(from Audiosurf 2/Audiosurf2_Data/Plugins/x86_64/)*  
* TagLib#  
* youtube-dl or a fork with the same API

#### Windows
The following libraries / programs are needed:

* BASS *(from the AS2 directory)*  
* ffmpeg  
* TagLib#  
* UnityMediaPlayer.dll *(from the AS2 directory)*  
* youtube-dl or a fork with the same API

### Troubleshooting
#### Linux
##### `This video is not available`
If youtube-dl exits with this message but the video is available, try 
the `-6` flag.

##### DllNotFoundException
In case of a DllNotFoundException, run the program with `MONO_LOG_LEVEL=debug` 
and `MONO_LOG_MASK=dll`. The output should tell you which libraries are missing.

You might also have to move `libASMedia` and `libbass` to `/usr/lib/`.

##### ALSA: `cannot find card 0`
You may need to install a [dummy soundcard](https://www.raspberrypi.org/forums/viewtopic.php?p=485842&sid=5b596e5473571e5918872059e32a6873#p485842) 
on your system.
