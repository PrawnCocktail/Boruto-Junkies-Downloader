## Boruto-Junkies Downloader

**How to use**   
Double click the exe and enter the url of the first episode you want to download, it will then download that episode and all subsequent episodes.   
For instance, entering the url "https://boruto-junkies.com/naruto-shippuuden-folge-1/" will download episode 1, epsiode 2, episode 3 etc etc. 

**What it does**  
- Downloads videos from Boruto-Junkies.   
- Downloads at the highest resolution available.   

**What it doesn't do**  
- Doesn't download subtitles.  
- Anything fast. No seriously, the servers are slow as shit and 1 episode takes far to long to download.   

Nothing i can do about the episode names, they are named as shown on the site.  

The ".ts" videos that this outputs will probably need to be run though ffmpeg for a better viewing experience.  
I Personally use this command:  
	ffmpeg -i "episode-name.ts" -acodec copy -vcodec copy "episode-name.mp4"  

"Hey, your code sucks!" Yup, thats why I do this for fun and not for a living.  