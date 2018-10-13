//------------------------------//
//  MusicTransitionFader.js     //
//  Written by Alucard Jay      //
//  2/11/2014                   //
//------------------------------//
 
#pragma strict
 
import System.Collections.Generic;
 
 
public var musicTracks : List.< AudioClip >; // drag and drop audio clips in Inspector * MINIMUM 2 clips *
 
public var playTimeMin : float = 60; // time in seconds, minimum time before changing tracks
public var playTimeMax : float = 180; // time in seconds, maximum time before changing tracks
 
public var transitionFadeTime : float = 3; // time in seconds, time it takes for fade to complete
public var transitionFadeVolume : float = 0.25; // volume between 0 and 1, volume the current track gets to before blending in the new track
 
 
private var audioSources : AudioSource[];
private var currentSource : int = 0;
 
private var musicTracksAvailable : List.< int >;
private var currentTrack : int = 0;
 
private var isFading : boolean = false;
 
private var timer : float = 0;
private var timerMax : float = 60;
 
 
//  Persistant Functions
//  ----------------------------------------------------------------------------
 
 
function Awake() 
{
    // NOTE : this object must have 2 child objects,
    // each with an AudioSource component to work
    audioSources = GetComponentsInChildren.< AudioSource >();
}
 
 
function Start() 
{
    // dummy select current track playing 
    // (so its possible that musicTracks[0] could be selected as first track to play)
    currentTrack = Random.Range( 0, musicTracks.Count );
 
    // set timer over max so first update is set to fading
    // after resetting values in CheckTrackTimer()
    timer = timerMax + 1;
}
 
 
function Update() 
{
    timer += Time.deltaTime;
 
    switch ( isFading )
    {
       case false :
         CheckTrackTimer();
       break;
 
       case true :
         FadeTransition();
       break;
    }
}
 
 
//  Other Functions
//  ----------------------------------------------------------------------------
 
 
function SelectNextTrack() 
{
    // create a list of available tracks to choose from
    // (so the current track is not chosen and repeated)
    musicTracksAvailable = new List.< int >();
 
    for ( var t : int = 0; t < musicTracks.Count; t ++ )
    {
       musicTracksAvailable.Add( t );
    }
 
    //remove current track from the list
    musicTracksAvailable.RemoveAt( currentTrack );
 
    // pick a new random track
    currentTrack = musicTracksAvailable[ Random.Range( 0, musicTracksAvailable.Count ) ];
 
    // assign track to AudioSource that is NOT currently playing
    
  	 	audioSources[ ( currentSource + 1 ) % 2 ].GetComponent.<AudioSource>().Stop();
    	audioSources[ ( currentSource + 1 ) % 2 ].GetComponent.<AudioSource>().clip = musicTracks[ currentTrack ];
    	audioSources[ ( currentSource + 1 ) % 2 ].GetComponent.<AudioSource>().volume = 0;
    //}
}
 
 
function CheckTrackTimer() 
{
    if ( timer > timerMax )
    {
       // reset timer, set new max time to next transition
       timer = 0;
       timerMax = Random.Range( playTimeMin, playTimeMax );
 
       SelectNextTrack();
 
       isFading = true;
    }
}
 
 
function FadeTransition() 
{
    var fadeIn : float = 0;
 
    // calculate the fade Out volume
    var fadeOut : float = 1.0 - ( timer / transitionFadeTime );
    fadeOut = Mathf.Clamp01( fadeOut );
 
    audioSources[ currentSource ].GetComponent.<AudioSource>().volume = fadeOut;
 
 
    // if fadeOut is low enough, start playing the new track and fade it in
 
    if ( fadeOut < ( transitionFadeVolume * 2.0 ) )
    {
       // is the next track playing yet?
       if ( !audioSources[ ( currentSource + 1 ) % 2 ].isPlaying )
       {
         audioSources[ ( currentSource + 1 ) % 2 ].GetComponent.<AudioSource>().Play();
       }
 
       // calculate the fade In volume
       fadeIn = timer;
       // minus how long for the other source to reach transitionFadeVolume
       fadeIn -= ( 1.0 - transitionFadeVolume ) * transitionFadeTime;
       // add how long for this source to reach transitionFadeVolume
       fadeIn += transitionFadeVolume * transitionFadeTime;
       // normalize by dividing by fade time
       fadeIn /= transitionFadeTime;
 
       fadeIn = Mathf.Clamp01( fadeIn );
 
       audioSources[ ( currentSource + 1 ) % 2 ].GetComponent.<AudioSource>().volume = fadeIn;
    }
 
 
    // check if next track has fully faded in
    if ( fadeIn == 1 )
    {
       // stop the old audio from playing
       audioSources[ currentSource ].GetComponent.<AudioSource>().Stop();
 
       // set new current AudioSource index
       currentSource = ( currentSource + 1 ) % 2;
 
       // reset timer
       timer = 0;
 
       isFading = false;
    }
 
    //Debug.Log( "fadeOut = " + fadeOut + " : fadeIn = " + fadeIn );
}