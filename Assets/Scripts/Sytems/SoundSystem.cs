using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using static MyUtility.Utility;

public class SoundSystem : Entity {


    private const string SFXBundleLabel = "SFXBundle";
    private const string tracksBundleLabel = "TracksBundle";

    private bool initializing = false;
    private bool loadingBundles = false;
    private bool loadingAudioClips = false;

    private bool canPlaySFX = false;
    private bool canPlayTracks = false;

    private AsyncOperationHandle<ScriptableObject> SFXBundleHandle;
    private AsyncOperationHandle<ScriptableObject> tracksBundleHandle;

    private Dictionary<string, AsyncOperationHandle<AudioClip>> loadedSFXAssetsHandles;
    private Dictionary<string, AsyncOperationHandle<AudioClip>> loadedTracksAssets;

    public override void Initialize(GameInstance game) {
        if (initialized || initializing)
            return;

        gameInstanceRef = game;
        initializing = true; //This was my last problem
        LoadBundles();
    }
    public override void Tick() {
        if (initializing)
            UpdateIntializingState();
        else if (canPlaySFX || canPlayTracks)
            UpdateNormalState();
    }
    public override void CleanUp(string message = "Entity cleaned up successfully!") {
        if (GameInstance.showSystemMessages)
            Log(message);

        UnloadResources();
    }

    private void UpdateIntializingState() {
        if (loadingBundles) {
            if (HasFinishedLoadingBundles())
                LoadAudioClips();
        }
        else if (loadingAudioClips) {
            if (HasFinishedLoadingAudioClips()) {
                if (GameInstance.showSystemMessages)
                    Log("SoundSystem has finished loading AudioClips");
                ConfirmInitialization();
            }
        }
        else
            Error("Was not supposed to be called!!!!!!");
    }
    private void UpdateNormalState() {

    }

    //Resource Management
    private void UnloadResources() {
        if (SFXBundleHandle.IsValid())
            Addressables.Release(SFXBundleHandle);
        if (tracksBundleHandle.IsValid())
            Addressables.Release(tracksBundleHandle);
    }
    private void LoadBundles() {
        if (GameInstance.showSystemMessages)
            Log("SoundSystem started loading bundles!");

        //Notes:
        //-All exceptions thrown by LoadAssetAsync are disabled.
        //-Should not crash the game if an exception is thrown.

        SFXBundleHandle = Addressables.LoadAssetAsync<ScriptableObject>(SFXBundleLabel);
        if (SFXBundleHandle.IsDone)
            Warning("SoundSystem failed to load SFXBundle\nReason: " + SFXBundleHandle.OperationException.Message + "\nPlaying SFX will not be possible!");
        else
            SFXBundleHandle.Completed += FinishedLoadingSFXBundleCallback;
        
        tracksBundleHandle = Addressables.LoadAssetAsync<ScriptableObject>(tracksBundleLabel);
        if (tracksBundleHandle.IsDone)
            Warning("SoundSystem failed to load TracksBundle\nReason: " + tracksBundleHandle.OperationException.Message + "\nPlaying tracks will not be possible!");
        else
            tracksBundleHandle.Completed += FinishedLoadingTracksBundleCallback;
        
        if (SFXBundleHandle.IsDone && tracksBundleHandle.IsDone) {
            Warning("SoundSystem will not be able to play any audio clips!");
            ConfirmInitialization();
        }
        else
            loadingBundles = true;
    }
    private void LoadAudioClips() {

        loadingAudioClips = false;

        if (canPlaySFX) {
            loadedSFXAssetsHandles = new Dictionary<string, AsyncOperationHandle<AudioClip>>();
            foreach(var asset in ((SFXBundle)SFXBundleHandle.Result).entries) {
                var handle = Addressables.LoadAssetAsync<AudioClip>(asset.clip);
                handle.Completed += FinishedLoadingAudioClipCallback;
                loadedSFXAssetsHandles.Add(asset.key, handle);
                if (GameInstance.showSystemMessages)
                    Log("SoundSystem started loading SFX audio clip with key [" + asset.key + "]");
            }
            loadingAudioClips = true;
        }
        if (canPlayTracks) {
            loadedTracksAssets = new Dictionary<string, AsyncOperationHandle<AudioClip>>();
            foreach(var asset in ((TracksBundle)tracksBundleHandle.Result).entries) {
                var handle = Addressables.LoadAssetAsync<AudioClip>(asset.clip);
                handle.Completed += FinishedLoadingAudioClipCallback;
                loadedTracksAssets.Add(asset.key, handle);
                if (GameInstance.showSystemMessages)
                    Log("SoundSystem started loading track audio clip with key [" + asset.key + "]");
            }
            loadingAudioClips = true;
        }

        
    }

    private bool HasFinishedLoadingBundles() {
        bool result = true;

        result &= SFXBundleHandle.IsDone;
        result &= tracksBundleHandle.IsDone;

        loadingBundles = !result;
        return result;
    }
    private bool HasFinishedLoadingAudioClips() {
        bool result = true;

        //The weird ordering is cause of this
        //Check individual handles saved in dictionary or list

        loadingAudioClips = result;
        return result;
    }

    private void ConfirmInitialization() {
        if (GameInstance.showSystemMessages)
            Log("SoundSystem has been initialized successfully!");

        initializing = false;
        initialized = true;
    }

    //User
    public bool PlayLocalSFX(string key) {

        return true;
    }
    public bool PlayGlobalSFX(string key) {

        return true;
    }
    public bool PlayTrack(string key) {

        return true;
    }
    public void StopTrack() {

    }


    //Callbacks
    private void FinishedLoadingTracksBundleCallback(AsyncOperationHandle<ScriptableObject> handle) {
        if (handle.Status == AsyncOperationStatus.Succeeded) {
            if (GameInstance.showSystemMessages)
                Log("SoundSystem finished loading TracksBundle successfully!");
            canPlayTracks = true;
        }
        else if (handle.Status == AsyncOperationStatus.Failed) {
            Warning("SoundSystem failed to load TracksBundle\nPlaying tracks will not be possible!");
        }
    }
    private void FinishedLoadingSFXBundleCallback(AsyncOperationHandle<ScriptableObject> handle) {
        if (handle.Status == AsyncOperationStatus.Succeeded) {
            if (GameInstance.showSystemMessages)
                Log("SoundSystem finished loading SFXBundle successfully!");
            canPlaySFX = true;
        }
        else if (handle.Status == AsyncOperationStatus.Failed) {
            Warning("SoundSystem failed to load SFXBundle\nPlaying SFX will not be possible!");
        }
    }
    private void FinishedLoadingAudioClipCallback(AsyncOperationHandle<AudioClip> handle) {
        if (handle.Status == AsyncOperationStatus.Succeeded) {
            if (GameInstance.showSystemMessages)
                Log("SoundSystem loaded audio clip [" + handle.Result.name + "] successfully!");
        }
        else if (handle.Status == AsyncOperationStatus.Failed)
            Warning("SoundSystem failed to load [" + handle.Result.name + "]\n Playing this audio clip will be possible!");
    }
}
