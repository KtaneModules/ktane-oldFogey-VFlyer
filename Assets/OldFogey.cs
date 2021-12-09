using Newtonsoft.Json.Bson;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
//using KModkit;
using rnd = UnityEngine.Random;
using KeepCoding;
//using System.Text.RegularExpressions;

public class OldFogey : MonoBehaviour
{
	public KMBombInfo bomb;
	public KMAudio Audio;
	public KMBombModule modSelf;
	public KMSelectable[] btns;
	public AudioSource audioPlayer;
	public AudioClip[]
		overrideTrack1Tape1, overrideTrack2Tape1, overrideTrack3Tape1, overrideTrack4Tape1, overrideTrack5Tape1,
		overrideTrack1Tape2, overrideTrack2Tape2, overrideTrack3Tape2, overrideTrack4Tape2, overrideTrack5Tape2,
		overrideTrack1Tape3, overrideTrack2Tape3, overrideTrack3Tape3, overrideTrack4Tape3, overrideTrack5Tape3,
		overrideTrack1Tape4, overrideTrack2Tape4, overrideTrack3Tape4, overrideTrack4Tape4, overrideTrack5Tape4,
		overrideTrack1Tape5, overrideTrack2Tape5, overrideTrack3Tape5, overrideTrack4Tape5, overrideTrack5Tape5;
	public KMSelectable submitBtn;
	public Material[] screenColors;
	public StatusLightFogey fakeStatusLight;
	public GameObject screen;
	public TextMesh display;

	RGBColor startColor;
	int correctTape = -1;
	int[] btnSounds;
	RGBColor[] btnColors;

	IEnumerator colorFlash;

	bool submit = false;
	List<int> presses;
	RGBColor currentColor;

	//Logging
	private static int moduleIdCounter = 1;
	private int moduleId;
	private bool moduleSolved, playSounds = true;

	int[] autoSolvePresses;


	string GetStartingColor()
    {
		return startColor.GetName();
    }

	void Awake()
	{
		moduleId = moduleIdCounter++;

		for (int x = 0; x < btns.Length; x++)
		{
			int y = x;
			btns[x].OnInteract += delegate
			{
				PressButton(y);
				return false;
			};
		}
		submitBtn.OnInteract += delegate () { HandleSubmit(); return false; };
	}

	void Start()
	{
		SetUpBtns();
		ResizeLights();
	}

	void PressButton(int btn)
	{
		btns[btn].AddInteractionPunch(.5f);

		if (moduleSolved)
		{
			PlaySound(btnSounds[btn]);
			return;
		}

		if (submit)
		{
			if (presses.Count() == 4)
				return;

			presses.Add(btn);
			display.text += GetButtonSymbol(btn);

			if (presses.Count() == 4)
			{
				CheckSolution();
			}
			else
			{
				PlaySound(btnSounds[btn]);
				currentColor = currentColor.Sum(btnColors[btn]);
				fakeStatusLight.ShowColor(currentColor.GetName());
			}
		}
		else
		{
			PlaySound(btnSounds[btn]);
			if (colorFlash != null)
				StopCoroutine(colorFlash);
			colorFlash = ColorFlash(btnColors[btn], false);
			StartCoroutine(colorFlash);
		}
	}

	void PlaySound(int n)
	{
		//int[] playIdxAlts = { 14 };
		if (playSounds)
		{
			/*
			if (n >= 0 && n < possibleSounds.Length)
			{
				//Debug.LogFormat("<Old Fogey #{0}> Attempting to play track {1} - tape {2}", moduleId, n / 5 + 1, n % 5 + 1);
				
				if (playIdxAlts.Contains(n) || true)
				{*/
					//Debug.LogFormat("<Old Fogey #{0}> Bypassing game sound handler with {1}", moduleId, alternativeSoundNames[n]);
					BypassKMAudio(n);
				/*
				}
				else
				{
					sound = Audio.PlayGameSoundAtTransformWithRef(possibleSounds[n], transform); // Note to self: method cannot by easily bypassed internally.
				}
				
			}
			else
				sound = null;
			soundStop = StopSound(sound);
			StartCoroutine(soundStop);
			*/
		}
	}

	void BypassKMAudio(int idx)
    {
		if (idx < 0 || idx >= 25) return;

		AudioClip[][] overrideAudioClips = {
			overrideTrack1Tape1, overrideTrack2Tape1, overrideTrack3Tape1, overrideTrack4Tape1, overrideTrack5Tape1,
			overrideTrack1Tape2, overrideTrack2Tape2, overrideTrack3Tape2, overrideTrack4Tape2, overrideTrack5Tape2,
			overrideTrack1Tape3, overrideTrack2Tape3, overrideTrack3Tape3, overrideTrack4Tape3, overrideTrack5Tape3,
			overrideTrack1Tape4, overrideTrack2Tape4, overrideTrack3Tape4, overrideTrack4Tape4, overrideTrack5Tape4,
			overrideTrack1Tape5, overrideTrack2Tape5, overrideTrack3Tape5, overrideTrack4Tape5, overrideTrack5Tape5,
		};

		if (overrideAudioClips[idx].Any())
		{
			audioPlayer.clip = overrideAudioClips[idx][rnd.Range(0, overrideAudioClips[idx].Length)];
            audioPlayer.volume = Game.PlayerSettings.SFXVolume / 100f;
			audioPlayer.Play();
		}
    }

	void SetUpBtns()
	{
		btnSounds = new int[] {-1, -1, -1, -1, -1, -1, -1, -1, -1, -1};
		btnColors = new RGBColor[10];

		int[] priority = Enumerable.Range(0, 10).ToList().OrderBy(x => rnd.Range(0, 1000)).ToArray();

		if(priority[3] == 0)
		{
			priority[3] = priority[4];
			priority[4] = 0;
		}

		int tempColor;
		do {tempColor = (rnd.Range(0, 2) * 100) + (rnd.Range(0, 2) * 10) + (rnd.Range(0, 2));} while (tempColor == 0);
		startColor = new RGBColor(tempColor);
		fakeStatusLight.ShowColor(startColor.GetName());
		Debug.LogFormat("[Old Fogey #{0}] Starting status light color: {1}.", moduleId, startColor.GetName());

		for(int i = 0; i < btnColors.Length; i++)
		{
			btnColors[i] = new RGBColor((rnd.Range(0, 2) * 100) + (rnd.Range(0, 2) * 10) + (rnd.Range(0, 2)));
		}

		correctTape = btnColors[0].GetTapeNumber();
		currentColor = startColor.Clone();
		RGBColor c = startColor.Clone();
		for(int i = 0; i < 4; i++)
			c = c.Sum(btnColors[priority[i]]);

		if(c.r)
			btnColors[priority[3]].r = !btnColors[priority[3]].r;
		if(!c.g)
			btnColors[priority[3]].g = !btnColors[priority[3]].g;
		if(c.b)
			btnColors[priority[3]].b = !btnColors[priority[3]].b;

		int[] tapes = Enumerable.Range(0, 5).ToList().OrderBy(x => rnd.Range(0, 1000)).ToArray();
		int[] tracks = Enumerable.Range(0, 5).ToList().OrderBy(x => rnd.Range(0, 1000)).ToArray();

		tapes[tapes.ToList().IndexOf(correctTape)] = tapes[4];
		tapes[4] = correctTape;

		for(int i = 0; i < 4; i++)
		{
			btnSounds[priority[i]] = tapes[i] * 5 + tracks[i];
		}

		btnSounds[priority[4]] = correctTape * 5 + rnd.Range(0, 5);
		btnSounds[priority[5]] = correctTape * 5 + rnd.Range(0, 5);

		for(int i = 6; i < priority.Length; i++)
		{
			int tape;
			
			do {tape = rnd.Range(0, 5); } while (tape == correctTape);

			List<int> prevTracks = new List<int>();
			List<RGBColor> prevColors = new List<RGBColor>();

			for(int j = 0; j < i; j++)
			{
				if(btnSounds[priority[j]] / 5 == tape)
				{
					prevTracks.Add(btnSounds[priority[j]] % 5);
					prevColors.Add(btnColors[priority[j]]);
				}
			}

			int track;
			do { track = rnd.Range(0, 5); } while (prevTracks.Exists(x => x == track));
			do {tempColor = (rnd.Range(0, 2) * 100) + (rnd.Range(0, 2) * 10) + (rnd.Range(0, 2));} while (prevColors.Exists(x => x.Equals(new RGBColor(tempColor))));
			
			btnSounds[priority[i]] = tape * 5 + track;
			if(priority[i] != 0)
				btnColors[priority[i]] = new RGBColor(tempColor);
		}

		for(int i = 0; i < btnColors.Length; i++)
        	Debug.LogFormat("[Old Fogey #{0}] Button {1}'s color: {2}.", moduleId, i+1, btnColors[i].GetName());

		for(int i = 0; i < btnSounds.Length; i++)
        	Debug.LogFormat("[Old Fogey #{0}] Button {1}'s sound: Tape {2} - Track {3}.", moduleId, i+1, (btnSounds[i] / 5) + 1, (btnSounds[i] % 5) + 1);

        Debug.LogFormat("[Old Fogey #{0}] Correct tape: tape {1}.", moduleId, correctTape + 1);
        Debug.LogFormat("[Old Fogey #{0}] One possible correct input sequence: {1}.", moduleId, priority.Select(a => a + 1).Take(4).Join());
		autoSolvePresses = priority.ToArray();
	}

	void HandleSubmit()
	{
		Audio.PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, transform);
		submitBtn.AddInteractionPunch(.5f);

		if(moduleSolved)
			return;
		if (submit)
        {
			submit = false;
			screen.GetComponentInChildren<Renderer>().material = screenColors[0];
			display.text = "";
			if (colorFlash != null)
				StopCoroutine(colorFlash);
			StartCoroutine(ColorFlash(new RGBColor(000), true));
			return;
		}
		submit = true;
		display.text = "";
		Audio.PlaySoundAtTransform("spark", transform);
		screen.GetComponentInChildren<Renderer>().material = screenColors[1];
		if (presses == null) presses = new List<int>();
		presses.Clear();
		currentColor = startColor.Clone();
	}

	void CheckSolution()
	{
		bool isAllCorrect = true;

		if(presses.Exists(x => btnSounds[x] / 5 == correctTape))
		{
        	Debug.LogFormat("[Old Fogey #{0}] At least one button sound must belongs to the correct tape. None of the buttons pressed when submitting belong to the correct tape.", moduleId);
			isAllCorrect = false;
		}

		for(int i = 0; i < presses.Count(); i++)
			for(int j = i + 1; j < presses.Count(); j++)
			{
				if(btnSounds[presses[i]] / 5 == btnSounds[presses[j]] / 5)
				{
					Debug.LogFormat("[Old Fogey #{0}] Buttons {1} and {2} belong to the same tape.", moduleId, presses.ElementAt(i) + 1, presses.ElementAt(j) + 1);
					isAllCorrect = false;
				}
				else if(btnSounds[presses[i]] % 5 == btnSounds[presses[j]] % 5)
				{
					Debug.LogFormat("[Old Fogey #{0}] Buttons {1} and {2} have the same track number.", moduleId, presses.ElementAt(i) + 1, presses.ElementAt(j) + 1);
					isAllCorrect = false;
				}
			}

		RGBColor c = startColor.Clone();
		for(int i = 0; i < presses.Count(); i++)
			c = c.Sum(btnColors[presses.ElementAt(i)]);
		
		if(!c.Equals(new RGBColor(010)))
		{
			Debug.LogFormat("[Old Fogey #{0}] Final color was {1}, not Green.", moduleId,  c.GetName());
			isAllCorrect = false;
		}
		if (!isAllCorrect)
        {
			Debug.LogFormat("[Old Fogey #{0}] Strike! Not all conditions were satisfied upon receiving these inputs: {1}.", moduleId, presses.Select(a => a + 1).Join());
			modSelf.HandleStrike();
			display.text = "";
			screen.GetComponentInChildren<Renderer>().material = screenColors[0];
			StartCoroutine(ColorFlash(new RGBColor(100), true));
			return;
		}


		moduleSolved = true;
		Debug.LogFormat("[Old Fogey #{0}] Received input: {1}. Module solved.", moduleId, presses.Select(a => a + 1).Join());
		for(int i = 0; i < 5; i++)
			if(!presses.Exists(x => btnSounds[x] % 5 == i))
				PlaySound(correctTape * 5 + i);

		fakeStatusLight.ShowColor("Green");
		modSelf.HandlePass();
	}

	string GetButtonSymbol(int btn)
	{
		switch(btn)
		{
			case 0: return "❖";
			case 1: return "✢";
			case 2: return "✠";
			case 3: return "✛";
			case 4: return "✤";
			case 5: return "✜";
			case 6: return "✥";
			case 7: return "✙";
			case 8: return "✚";
			case 9: return "✣";
		}

		return "";
	}

	void ResizeLights()
	{
		float scalar = fakeStatusLight.transform.lossyScale.x;

		for(int i = 0; i < 8; i++)
			foreach(Light l in fakeStatusLight.transform.GetChild(i).GetComponentsInChildren<Light>())
				l.range *= scalar;
	}

	IEnumerator StopSound(KMAudio.KMAudioRef givenSound)
	{
		yield return new WaitForSeconds(4f);
		if (givenSound != null)
			givenSound.StopSound();
	}

	IEnumerator ColorFlash(RGBColor color, bool restore = false)
	{
		fakeStatusLight.ShowColor(color.GetName());

		yield return new WaitForSeconds(1f);

		if (restore)
		{
			currentColor = startColor.Clone();
			submit = false;
		}
		fakeStatusLight.ShowColor(currentColor.GetName());
	}

	//Twitch Plays Handling
	readonly string[] validInputs = { "1", "2", "3", "4", "5", "6", "7", "8", "9", "10" };
	private bool cmdIsValid(string param)
    {
        string[] parameters = param.Split(' ', ',');
        for (int i = 1; i < parameters.Length; i++)
        {
            if (!validInputs.Contains(parameters[i]))
            {
                return false;
            }
        }
        return true;
    }

	IEnumerator TwitchHandleForcedSolve()
	{
		Debug.LogFormat("[Old Fogey #{0}] Force solve issued viva TP handler.", moduleId);
		if (submit && presses.Any())
		{
			submitBtn.OnInteract();
			yield return new WaitForSeconds(1.1f);
			submitBtn.OnInteract();
			yield return new WaitForSeconds(0.1f);
		}
		else if (!submit)
		{
			submitBtn.OnInteract();
			yield return new WaitForSeconds(0.1f);
		}
		//playSounds = false;
		for (int x = 0; x < autoSolvePresses.Length; x++)
		{
			btns[autoSolvePresses[x]].OnInteract();
			//sound.StopSound();
			if (x + 1 < autoSolvePresses.Length)
			yield return new WaitForSeconds(0.1f);
		}
	}

	#pragma warning disable 414
	private readonly string TwitchHelpMessage = "Press the following buttons with \"!{0} press 1\" Valid buttons for the press command are 1-10; 1 representing the ❖ button and its variants in reading order. Multiple buttons presses can be chained I.E \"!{0} press 1 2 3 4 5...\" Press the submit button with \"!{0} submit\" Reset inputs while submitting with \"!{0} reset\" Adjust the press delay on the module with \"!{0} pressdelay #.##\" from .25 of a second to 4 seconds per press. This does not apply when submitting.";
#pragma warning restore 414

	private float pressDelay = .5f;
    IEnumerator ProcessTwitchCommand(string command)
    {
		command = command.ToLower();
        if (command.RegexMatch(@"^reset$"))
        {
			if (!submit)
			{
				yield return "sendtochaterror The module is not submitting yet. You can only reset inputs if the submit button is pressed.";
				yield break;
			}
            yield return null;
			submitBtn.OnInteract();
		}
		else if (command.RegexMatch(@"^submit$"))
        {
			if (submit)
			{
				yield return "sendtochaterror The module is already in submit mode. If you want to reset submission, do \"!{1} reset\" instead.";
				yield break;
			}
			yield return null;
            submitBtn.OnInteract();
        }
		else if (command.RegexMatch(@"^pressdelay\s\d*(.\d+)?$"))
		{
			if (submit)
			{
				yield return "sendtochaterror The module is submitting. This essentially does nothing unless you reset submission.";
				yield break;
			}
            float handledPressDelay;
            if (!float.TryParse(command.Substring(10).Trim(), out handledPressDelay) || handledPressDelay < 0.25f || handledPressDelay > 4f)
            {
				yield return string.Format("sendtochaterror I am unable to set the press delay for Old Fogey (#{0}) to \"{1}\" second(s).","{1}", command.Substring(11));
				yield break;
			}
			yield return null;
			pressDelay = handledPressDelay;
			yield return string.Format("sendtochat Press delay for Old Fogey (#{0}) has been set to \"{1}\" second(s).", "{1}", handledPressDelay.ToString("0.00"));
			yield break;
		}
		else if (command.RegexMatch(@"^press\s\d{1,2}((\s|,)\s?\d{1,2})*$"))
		{
			List<KMSelectable> selectablesTP = new List<KMSelectable>();
			string[] possibleItems = command.Replace("press", "").Trim().Split();
			foreach (string currentItem in possibleItems)
			{
				if (validInputs.Contains(currentItem.Trim()))
				{
					selectablesTP.Add(btns[Array.IndexOf(validInputs, currentItem)]);
				}
				else
				{
					yield return "sendtochaterror Sorry but \""+currentItem+"\" is not a valid button.";
					yield break;
				}
			}
			for (int x = 0; x < selectablesTP.Count; x++)
			{
				yield return null;
				selectablesTP[x].OnInteract();
				float timeSpent = 0f;
				do
				{
					yield return string.Format("trycancel Your button interaction has been canceled after {0} press{1}.", x, x == 1 ? "" : "es");
					yield return null;
					timeSpent += Time.deltaTime;
				}
				while (timeSpent < (submit ? 0.1f : pressDelay));

				
			}
		}
		yield break;
	}
}
