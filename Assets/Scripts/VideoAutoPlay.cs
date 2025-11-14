using UnityEngine;
using UnityEngine.Video;

public class VideoAutoPlay : MonoBehaviour
{
    private VideoPlayer videoPlayer;

    void Start()
    {
        videoPlayer = GetComponent<VideoPlayer>();
        videoPlayer.isLooping = true;
        videoPlayer.Play();
    }

    void OnEnable()
    {
        if (videoPlayer != null && !videoPlayer.isPlaying)
        {
            videoPlayer.Play();
        }
    }
}
