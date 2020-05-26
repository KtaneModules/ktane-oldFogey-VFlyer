﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
//using KModkit;
using rnd = UnityEngine.Random;
//using System.Text.RegularExpressions;

public class OldFogey : MonoBehaviour 
{
	public KMBombInfo bomb;
	public KMAudio Audio;

	public KMSelectable[] btns;
	public KMSelectable submitBtn;
	public Material[] screenColors;
	public GameObject fakeStatusLight;
	public GameObject screen;
	public TextMesh display;

	RGBColor startColor;
	int correctTape = -1;
	int[] btnSounds;
	RGBColor[] btnColors;

	Coroutine colorFlash, soundStop;
	KMAudio.KMAudioRef sound;

	bool submit = false;
	List<int> presses;
	RGBColor currentColor;

	//Logging
	static int moduleIdCounter = 1;
    int moduleId;
    private bool moduleSolved;

	int[] autoSolvePresses;

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

	void Start () 
	{
		SetUpBtns();
		ResizeLights();
	}

	void PressButton(int btn)
	{
		btns[btn].AddInteractionPunch(.5f);

		if(moduleSolved)
		{
			PlaySound(btnSounds[btn]);
			return;
		}

		if(submit)
		{
			if(presses.Count() == 4)
				return;

			presses.Add(btn);
			display.text += GetButtonSymbol(btn);
			
			if(presses.Count() == 4)
			{
				CheckSolution();
			}
			else
			{
				PlaySound(btnSounds[btn]);
				currentColor = currentColor.Sum(btnColors[btn]);
				for(int i = 0; i < fakeStatusLight.transform.childCount; i++)
					fakeStatusLight.transform.GetChild(i).gameObject.SetActive(false);
				fakeStatusLight.transform.Find(currentColor.GetName()).gameObject.SetActive(true);
			}
		}	
		else
		{
			PlaySound(btnSounds[btn]);
			if(colorFlash != null)
				StopCoroutine(colorFlash);
			colorFlash = StartCoroutine(ColorFlash(btnColors[btn], false));
		}	
	}

	void PlaySound(int n)
	{
		if(soundStop != null)
		{
			StopCoroutine(soundStop);
			if (sound != null)
				sound.StopSound();
		}
		//Debug.LogFormat("[Old Fogey #{0}] Attempting to play track {1} - tape {2}", moduleId, n / 5 + 1, n % 5 + 1);

		KMSoundOverride.SoundEffect[] possibleSounds = {
			// Tape 1
			KMSoundOverride.SoundEffect.ButtonPress,
			KMSoundOverride.SoundEffect.BigButtonRelease,
			KMSoundOverride.SoundEffect.WireSnip,
			KMSoundOverride.SoundEffect.Strike,
			KMSoundOverride.SoundEffect.WireSequenceMechanism,
			// Tape 2
			KMSoundOverride.SoundEffect.NeedyActivated,
			KMSoundOverride.SoundEffect.NeedyWarning,
			KMSoundOverride.SoundEffect.EmergencyAlarm,
			KMSoundOverride.SoundEffect.MenuButtonPressed,
			KMSoundOverride.SoundEffect.TitleMenuPressed,
			// Tape 3
			KMSoundOverride.SoundEffect.FastestTimerBeep,
			KMSoundOverride.SoundEffect.CapacitorPop,
			KMSoundOverride.SoundEffect.LightBuzz,
			KMSoundOverride.SoundEffect.LightBuzzShort,
			KMSoundOverride.SoundEffect.SelectionTick,
			// Tape 4
			KMSoundOverride.SoundEffect.BombDefused,
			KMSoundOverride.SoundEffect.BombExplode,
			KMSoundOverride.SoundEffect.CorrectChime,
			KMSoundOverride.SoundEffect.TypewriterKey,
			KMSoundOverride.SoundEffect.GameOverFanfare,
			// Tape 5
			KMSoundOverride.SoundEffect.Switch,
			KMSoundOverride.SoundEffect.Stamp,
			KMSoundOverride.SoundEffect.BombDrop,
			KMSoundOverride.SoundEffect.BriefcaseOpen,
			KMSoundOverride.SoundEffect.PageTurn,
		};
		string[] alternativeSoundNames = {
			"Tape1Track1",
			"Tape1Track2",
			"Tape1Track3",
			"Tape1Track4",
			"Tape1Track5",
			"Tape2Track1",
			"Tape2Track2",
			"Tape2Track3",
			"Tape2Track4",
			"Tape2Track5",
			"Tape3Track1",
			"Tape3Track2",
			"Tape3Track3",
			"Tape3Track4",
			"Tape3Track5",
			"Tape4Track1",
			"Tape4Track2",
			"Tape4Track3",
			"Tape4Track4",
			"Tape4Track5",
			"Tape5Track1",
			"Tape5Track2",
			"Tape5Track3",
			"Tape5Track4",
			"Tape5Track5"
		};
		if (n >= 0 && n < possibleSounds.Length)
		{
			try
			{
				sound = Audio.PlayGameSoundAtTransformWithRef(possibleSounds[n], transform); // Note to self: method cannot by easily bypassed.
			}
			finally
			{
				
			}
		}
		else
			sound = null;
		soundStop = StartCoroutine(StopSound(sound));
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
		fakeStatusLight.transform.Find(startColor.GetName()).gameObject.SetActive(true);
        Debug.LogFormat("[Old Fogey #{0}] Starting status light color is {1}.", moduleId, startColor.GetName());

		for(int i = 0; i < btnColors.Length; i++)
		{
			btnColors[i] = new RGBColor((rnd.Range(0, 2) * 100) + (rnd.Range(0, 2) * 10) + (rnd.Range(0, 2)));
		}

		correctTape = btnColors[0].GetTapeNumber();

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
        	Debug.LogFormat("[Old Fogey #{0}] Button {1} color is {2}.", moduleId, i+1, btnColors[i].GetName());

		for(int i = 0; i < btnSounds.Length; i++)
        	Debug.LogFormat("[Old Fogey #{0}] Button {1} sound is Tape {2} - Track {3}.", moduleId, i+1, (btnSounds[i] / 5) + 1, (btnSounds[i] % 5) + 1);

        Debug.LogFormat("[Old Fogey #{0}] Correct tape is tape {1}.", moduleId, correctTape + 1);
        Debug.LogFormat("[Old Fogey #{0}] Example of correct input sequence: {1}.", moduleId, priority.Select(a => a + 1).Take(4).Join());
		autoSolvePresses = priority.ToArray();
	}

	void HandleSubmit()
	{
		Audio.PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, transform);
		submitBtn.AddInteractionPunch(.5f);

		if(moduleSolved || submit)
			return;

		submit = true;
		display.text = "";
		Audio.PlaySoundAtTransform("spark", transform);
		screen.GetComponentInChildren<Renderer>().material = screenColors[1];
		presses = new List<int>();
		currentColor = startColor.Clone();
	}

	void CheckSolution()
	{
		if(presses.Exists(x => btnSounds[x] / 5 == correctTape))
		{
        	Debug.LogFormat("[Old Fogey #{0}] Strike! Recieved input: {1}. At least one button sound belongs to the correct tape.", moduleId, presses.Select(a => a + 1).Join());
			GetComponent<KMBombModule>().HandleStrike();
			display.text = "";
			screen.GetComponentInChildren<Renderer>().material = screenColors[0];
			StartCoroutine(ColorFlash(new RGBColor(100), true));
			return;
		}

		for(int i = 0; i < presses.Count(); i++)
			for(int j = i + 1; j < presses.Count(); j++)
			{
				if(btnSounds[presses[i]] / 5 == btnSounds[presses[j]] / 5)
				{
					Debug.LogFormat("[Old Fogey #{0}] Strike! Recieved input: {1}. Buttons {2} and {3} belong to the same tape.", moduleId, presses.Select(a => a + 1).Join(), presses.ElementAt(i) + 1, presses.ElementAt(j) + 1);
					GetComponent<KMBombModule>().HandleStrike();
					display.text = "";
					screen.GetComponentInChildren<Renderer>().material = screenColors[0];
					StartCoroutine(ColorFlash(new RGBColor(100), true));
					return;
				}
				else if(btnSounds[presses[i]] % 5 == btnSounds[presses[j]] % 5)
				{
					Debug.LogFormat("[Old Fogey #{0}] Strike! Recieved input: {1}. Buttons {2} and {3} have the same track number.", moduleId, presses.Select(a => a + 1).Join(), presses.ElementAt(i) + 1, presses.ElementAt(j) + 1);
					GetComponent<KMBombModule>().HandleStrike();
					display.text = "";
					screen.GetComponentInChildren<Renderer>().material = screenColors[0];
					StartCoroutine(ColorFlash(new RGBColor(100), true));
					return;
				}
			}

		RGBColor c = startColor.Clone();
		for(int i = 0; i < presses.Count(); i++)
			c = c.Sum(btnColors[presses.ElementAt(i)]);
		
		if(!c.Equals(new RGBColor(10)))
		{
			Debug.LogFormat("[Old Fogey #{0}] Strike! Recieved input: {1}. Final color was {2}, not Green.", moduleId, presses.Select(a => a + 1).Join(), c.GetName());
			GetComponent<KMBombModule>().HandleStrike();
			display.text = "";
			screen.GetComponentInChildren<Renderer>().material = screenColors[0];
			StartCoroutine(ColorFlash(new RGBColor(100), true));
			return;
		}

		moduleSolved = true;
		Debug.LogFormat("[Old Fogey #{0}] Recieved input: {1}. Module solved.", moduleId, presses.Select(a => a + 1).Join());
		for(int i = 0; i < 5; i++)
			if(!presses.Exists(x => btnSounds[x] % 5 == i))
				PlaySound(correctTape * 5 + i);

		for(int i = 0; i < fakeStatusLight.transform.childCount; i++)
			fakeStatusLight.transform.GetChild(i).gameObject.SetActive(false);
		fakeStatusLight.transform.Find(new RGBColor(10).GetName()).gameObject.SetActive(true);
		GetComponent<KMBombModule>().HandlePass();
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

	IEnumerator ColorFlash(RGBColor color, bool restore)
	{
		for(int i = 0; i < fakeStatusLight.transform.childCount; i++)
			fakeStatusLight.transform.GetChild(i).gameObject.SetActive(false);

		fakeStatusLight.transform.Find(color.GetName()).gameObject.SetActive(true);

		yield return new WaitForSeconds(1f);

		for(int i = 0; i < fakeStatusLight.transform.childCount; i++)
			fakeStatusLight.transform.GetChild(i).gameObject.SetActive(false);
		fakeStatusLight.transform.Find(startColor.GetName()).gameObject.SetActive(true);

		if(restore)
			submit = false;
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
		if (!submit)
		{
			submitBtn.OnInteract();
			yield return new WaitForSeconds(0.05f);
		}
		for (int x = 0; x < autoSolvePresses.Length; x++)
		{
			btns[autoSolvePresses[x]].OnInteract();
			sound.StopSound();
			yield return new WaitForSeconds(0.05f);
		}

		yield return true;

	}

	#pragma warning disable 414
	private readonly string TwitchHelpMessage = "Press the following buttons with \"!{0} press 1\" Valid buttons for the press command are 1-10; 1 representing the ❖ button and its variants in reading order. Multiple buttons presses can be chained I.E \"!{0} press 1 2 3 4 5...\" Press the submit button with \"!{0} submit\" Reset inputs while submitting with \"!{0} reset\"";
    #pragma warning restore 414

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
            Debug.LogFormat("[Old Fogey #{0}] Reset of inputs triggered! (TP)", moduleId);
            presses.Clear();
            submit = false;
            display.text = "";
            screen.GetComponentInChildren<Renderer>().material = screenColors[0];
            if (colorFlash != null)
                StopCoroutine(colorFlash);
        }
		else if (command.RegexMatch(@"^submit$"))
        {
            yield return null;
            submitBtn.OnInteract();

        }
		else if (command.RegexMatch(@"^press\s\d{1,2}((\s|,)\s?\d{1,2})?$"))
		{
			List<KMSelectable> selectablesTP = new List<KMSelectable>();
			string[] possibleItems = command.Replace("press ", "").Split();
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
				yield return "trycancel Your button interaction has been canceled.";
				selectablesTP[x].OnInteract();
				yield return new WaitForSeconds(submit ? 0.1f : 1f);
			}
		}
		yield break;
	}
}
