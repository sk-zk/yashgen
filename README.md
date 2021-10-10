Creates a YouTube ASH file for Audiosurf 2 from the command line.

## Usage
    yashgen video_id [options]

    Options:
      -d, --dest=VALUE           The output folder.
                                   Default: .
      -p, --proc=VALUE           Path to youtube-dl or a compatible fork.
                                   Default: ./youtube-dl
      -6                         Forces the downloader to use IPv6.
      -v, --verbose              Displays downloader console output.

## Installation
### Linux
Mono must be installed to run this program. Additionally, the following libraries / programs are needed:

* ffmpeg
* libASMedia.so *(from Audiosurf 2/Audiosurf2_Data/Plugins/x86_64/)*  
* libbass.so *(from Audiosurf 2/Audiosurf2_Data/Plugins/x86_64/)*  
* youtube-dl or a compatible fork

### Windows
The following libraries / programs are needed:

* BASS *(from the AS2 directory)*  
* ffmpeg   
* UnityMediaPlayer.dll *(from the AS2 directory)*  
* youtube-dl or a compatible fork

## Troubleshooting
### Linux
#### `This video is not available`
If youtube-dl exits with this message but the video is available, try 
the `-6` flag.

#### DllNotFoundException
In case of a DllNotFoundException, run the program with `MONO_LOG_LEVEL=debug` 
and `MONO_LOG_MASK=dll`. The output should tell you which libraries are missing.

You may also have to move `libASMedia` and `libbass` to `/usr/lib/`.

#### ALSA: `cannot find card 0`
You may need to install a [dummy soundcard](https://www.raspberrypi.org/forums/viewtopic.php?p=485842&sid=5b596e5473571e5918872059e32a6873#p485842) 
on your system.
