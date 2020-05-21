## Boruto-Junkies Downloader

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