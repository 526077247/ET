#import<AVFoundation/AVFoundation.h>

static AVAudioPlayer* audioPlayer;

extern "C" void* AddBackgroundAudioPlayer()
{	
	if(audioPlayer == nil)
	{
		AVAudioSession *audioSession = [AVAudioSession sharedInstance];
		[audioSession setCategory:AVAudioSessionCategoryPlayback error:nil];
		NSString *filePath = [[NSBundle mainBundle] pathForResource:@"BackgroundDownload" ofType:@"mp3"];
		NSData *fileData = [NSData dataWithContentsOfFile:filePath];
		
		audioPlayer = [[AVAudioPlayer alloc] initWithData:fileData error:nil];
		[audioPlayer setNumberOfLoops:-1];
		[audioPlayer prepareToPlay];
		[audioPlayer play];
	}
}

extern "C" void RemoveBackgroundAudioPlayer()
{
   if(audioPlayer != nil)
	{
		[audioPlayer stop];
		audioPlayer = nil;
	}
}