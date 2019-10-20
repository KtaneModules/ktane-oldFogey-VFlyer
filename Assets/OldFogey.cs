using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using KModkit;
using rnd = UnityEngine.Random;
using System.Text.RegularExpressions;

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

	Coroutine colorFlash;
	Coroutine soundStop;
	KMAudio.KMAudioRef sound;

	bool submit = false;
	List<int> presses;
	RGBColor currentColor;

	//Logging
	static int moduleIdCounter = 1;
    int moduleId;
    private bool moduleSolved;

	void Awake()
	{
		moduleId = moduleIdCounter++;
		GetComponent<KMBombModule>().OnActivate += Activate;

		btns[0].OnInteract += delegate () { PressButton(0); return false; };
		btns[1].OnInteract += delegate () { PressButton(1); return false; };
		btns[2].OnInteract += delegate () { PressButton(2); return false; };
		btns[3].OnInteract += delegate () { PressButton(3); return false; };
		btns[4].OnInteract += delegate () { PressButton(4); return false; };
		btns[5].OnInteract += delegate () { PressButton(5); return false; };
		btns[6].OnInteract += delegate () { PressButton(6); return false; };
		btns[7].OnInteract += delegate () { PressButton(7); return false; };
		btns[8].OnInteract += delegate () { PressButton(8); return false; };
		btns[9].OnInteract += delegate () { PressButton(9); return false; };
		submitBtn.OnInteract += delegate () { HandleSubmit(); return false; };
	}

	void Activate()
	{
		
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
			sound.StopSound();

		if(soundStop != null)
			StopCoroutine(soundStop);

		switch(n)
		{
			case 0: sound = GetComponent<KMAudio>().PlayGameSoundAtTransformWithRef(KMSoundOverride.SoundEffect.ButtonPress, transform); break;
			case 1: sound = GetComponent<KMAudio>().PlayGameSoundAtTransformWithRef(KMSoundOverride.SoundEffect.BigButtonRelease, transform); break;
			case 2: sound = GetComponent<KMAudio>().PlayGameSoundAtTransformWithRef(KMSoundOverride.SoundEffect.WireSnip, transform); break;
			case 3: sound = GetComponent<KMAudio>().PlayGameSoundAtTransformWithRef(KMSoundOverride.SoundEffect.Strike, transform); break;
			case 4: sound = GetComponent<KMAudio>().PlayGameSoundAtTransformWithRef(KMSoundOverride.SoundEffect.WireSequenceMechanism, transform); break;

			case 5: sound = GetComponent<KMAudio>().PlayGameSoundAtTransformWithRef(KMSoundOverride.SoundEffect.NeedyActivated, transform); break;
			case 6: sound = GetComponent<KMAudio>().PlayGameSoundAtTransformWithRef(KMSoundOverride.SoundEffect.NeedyWarning, transform); break;
			case 7: sound = GetComponent<KMAudio>().PlayGameSoundAtTransformWithRef(KMSoundOverride.SoundEffect.EmergencyAlarm, transform); break;
			case 8: sound = GetComponent<KMAudio>().PlayGameSoundAtTransformWithRef(KMSoundOverride.SoundEffect.AlarmClockBeep, transform); break;
			case 9: sound = GetComponent<KMAudio>().PlayGameSoundAtTransformWithRef(KMSoundOverride.SoundEffect.AlarmClockSnooze, transform); break;

			case 10: sound = GetComponent<KMAudio>().PlayGameSoundAtTransformWithRef(KMSoundOverride.SoundEffect.FastestTimerBeep, transform); break;
			case 11: sound = GetComponent<KMAudio>().PlayGameSoundAtTransformWithRef(KMSoundOverride.SoundEffect.CapacitorPop, transform); break;
			case 12: sound = GetComponent<KMAudio>().PlayGameSoundAtTransformWithRef(KMSoundOverride.SoundEffect.LightBuzz, transform); break;
			case 13: sound = GetComponent<KMAudio>().PlayGameSoundAtTransformWithRef(KMSoundOverride.SoundEffect.LightBuzzShort, transform); break;
			case 14: sound = GetComponent<KMAudio>().PlayGameSoundAtTransformWithRef(KMSoundOverride.SoundEffect.SelectionTick, transform); break;

			case 15: sound = GetComponent<KMAudio>().PlayGameSoundAtTransformWithRef(KMSoundOverride.SoundEffect.BombDefused, transform); break;
			case 16: sound = GetComponent<KMAudio>().PlayGameSoundAtTransformWithRef(KMSoundOverride.SoundEffect.BombExplode, transform); break;
			case 17: sound = GetComponent<KMAudio>().PlayGameSoundAtTransformWithRef(KMSoundOverride.SoundEffect.CorrectChime, transform); break;
			case 18: sound = GetComponent<KMAudio>().PlayGameSoundAtTransformWithRef(KMSoundOverride.SoundEffect.TypewriterKey, transform); break;
			case 19: sound = GetComponent<KMAudio>().PlayGameSoundAtTransformWithRef(KMSoundOverride.SoundEffect.GameOverFanfare, transform); break;

			case 20: sound = GetComponent<KMAudio>().PlayGameSoundAtTransformWithRef(KMSoundOverride.SoundEffect.Switch, transform); break;
			case 21: sound = GetComponent<KMAudio>().PlayGameSoundAtTransformWithRef(KMSoundOverride.SoundEffect.Stamp, transform); break;
			case 22: sound = GetComponent<KMAudio>().PlayGameSoundAtTransformWithRef(KMSoundOverride.SoundEffect.BombDrop, transform); break;
			case 23: sound = GetComponent<KMAudio>().PlayGameSoundAtTransformWithRef(KMSoundOverride.SoundEffect.BriefcaseOpen, transform); break;
			case 24: sound = GetComponent<KMAudio>().PlayGameSoundAtTransformWithRef(KMSoundOverride.SoundEffect.PageTurn, transform); break;
		}

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
        Debug.LogFormat("[Old Fogey #{0}] Example of correct input sequence is {1} {2} {3} {4}.", moduleId, priority[0] + 1, priority[1] + 1, priority[2] + 1, priority[3] + 1);
	}

	void HandleSubmit()
	{
		GetComponent<KMAudio>().PlayGameSoundAtTransform(KMSoundOverride.SoundEffect.ButtonPress, transform);
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
        	Debug.LogFormat("[Old Fogey #{0}] Strike! Recieved input: {1} {2} {3} {4}. At least one button sound belongs to the correct tape.", moduleId, presses.ElementAt(0) + 1, presses.ElementAt(1) + 1, presses.ElementAt(2) + 1, presses.ElementAt(3) + 1);
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
					Debug.LogFormat("[Old Fogey #{0}] Strike! Recieved input: {1} {2} {3} {4}. Buttons {5} and {6} belong to the same tape.", moduleId, presses.ElementAt(0) + 1, presses.ElementAt(1) + 1, presses.ElementAt(2) + 1, presses.ElementAt(3) + 1, presses.ElementAt(i) + 1, presses.ElementAt(j) + 1);
					GetComponent<KMBombModule>().HandleStrike();
					display.text = "";
					screen.GetComponentInChildren<Renderer>().material = screenColors[0];
					StartCoroutine(ColorFlash(new RGBColor(100), true));
					return;
				}
				else if(btnSounds[presses[i]] % 5 == btnSounds[presses[j]] % 5)
				{
					Debug.LogFormat("[Old Fogey #{0}] Strike! Recieved input: {1} {2} {3} {4}. Buttons {5} and {6} have the same track number.", moduleId, presses.ElementAt(0) + 1, presses.ElementAt(1) + 1, presses.ElementAt(2) + 1, presses.ElementAt(3) + 1, presses.ElementAt(i) + 1, presses.ElementAt(j) + 1);
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
			Debug.LogFormat("[Old Fogey #{0}] Strike! Recieved input: {1} {2} {3} {4}. Final color was {5}, not Green.", moduleId, presses.ElementAt(0) + 1, presses.ElementAt(1) + 1, presses.ElementAt(2) + 1, presses.ElementAt(3) + 1, c.GetName());
			GetComponent<KMBombModule>().HandleStrike();
			display.text = "";
			screen.GetComponentInChildren<Renderer>().material = screenColors[0];
			StartCoroutine(ColorFlash(new RGBColor(100), true));
			return;
		}

		moduleSolved = true;
		Debug.LogFormat("[Old Fogey #{0}] Recieved input: {1} {2} {3} {4}. Module solved.", moduleId, presses.ElementAt(0) + 1, presses.ElementAt(1) + 1, presses.ElementAt(2) + 1, presses.ElementAt(3) + 1);
		for(int i = 0; i < 5; i++)
			if(!presses.Exists(x => btnSounds[x] % 5 == i))
				PlaySound(correctTape * 5 + i);

		for(int i = 0; i < fakeStatusLight.transform.childCount; i++)
			fakeStatusLight.transform.GetChild(i).gameObject.SetActive(false);
		fakeStatusLight.transform.Find(new RGBColor(10).GetName()).gameObject.SetActive(true);
		GetComponent<KMBombModule>().HandlePass();
	}

	String GetButtonSymbol(int btn)
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

	IEnumerator StopSound(KMAudio.KMAudioRef sound)
	{
		yield return new WaitForSeconds(4f);
		sound.StopSound();
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

    //twitch plays
    private bool cmdIsValid(string param)
    {
        string[] parameters = param.Split(' ', ',');
        for (int i = 1; i < parameters.Length; i++)
        {
            if (!parameters[i].EqualsIgnoreCase("1") && !parameters[i].EqualsIgnoreCase("2") && !parameters[i].EqualsIgnoreCase("3") && !parameters[i].EqualsIgnoreCase("4") && !parameters[i].EqualsIgnoreCase("5") && !parameters[i].EqualsIgnoreCase("6") && !parameters[i].EqualsIgnoreCase("7") && !parameters[i].EqualsIgnoreCase("8") && !parameters[i].EqualsIgnoreCase("9") && !parameters[i].EqualsIgnoreCase("10"))
            {
                return false;
            }
        }
        return true;
    }

    #pragma warning disable 414
    private readonly string TwitchHelpMessage = @"!{0} press <button> [Presses the specified button] | !{0} press <button <button> [Example of button press chaining] | !{0} submit [Presses the submit button] | !{0} reset [Resets all inputs] | Valid buttons for the press command are 1-10, representing the buttons in reading order";
    #pragma warning restore 414

    IEnumerator ProcessTwitchCommand(string command)
    {
        if (Regex.IsMatch(command, @"^\s*reset\s*$", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant))
        {
            yield return null;
            Debug.LogFormat("[Old Fogey #{0}] Reset of inputs triggered! (TP)", moduleId);
            presses = new List<int>();
            submit = false;
            display.text = "";
            screen.GetComponentInChildren<Renderer>().material = screenColors[0];
            if (colorFlash != null)
                StopCoroutine(colorFlash);
            yield break;
        }
        if (Regex.IsMatch(command, @"^\s*submit\s*$", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant))
        {
            yield return null;
            submitBtn.OnInteract();
            yield break;
        }
        string[] parameters = command.Split(' ');
        if (Regex.IsMatch(parameters[0], @"^\s*press\s*$", RegexOptions.IgnoreCase | RegexOptions.CultureInvariant))
        {
            if (parameters.Length > 1)
            {
                if (cmdIsValid(command))
                {
                    yield return null;
                    for (int i = 1; i < parameters.Length; i++)
                    {
                        if (parameters[i].EqualsIgnoreCase("1"))
                        {
                            btns[0].OnInteract();
                        }
                        else if (parameters[i].EqualsIgnoreCase("2"))
                        {
                            btns[1].OnInteract();
                        }
                        else if (parameters[i].EqualsIgnoreCase("3"))
                        {
                            btns[2].OnInteract();
                        }
                        else if (parameters[i].EqualsIgnoreCase("4"))
                        {
                            btns[3].OnInteract();
                        }
                        else if (parameters[i].EqualsIgnoreCase("5"))
                        {
                            btns[4].OnInteract();
                        }
                        else if (parameters[i].EqualsIgnoreCase("6"))
                        {
                            btns[5].OnInteract();
                        }
                        else if (parameters[i].EqualsIgnoreCase("7"))
                        {
                            btns[6].OnInteract();
                        }
                        else if (parameters[i].EqualsIgnoreCase("8"))
                        {
                            btns[7].OnInteract();
                        }
                        else if (parameters[i].EqualsIgnoreCase("9"))
                        {
                            btns[8].OnInteract();
                        }
                        else if (parameters[i].EqualsIgnoreCase("10"))
                        {
                            btns[9].OnInteract();
                        }
                        yield return new WaitForSeconds(1.0f);
                    }
                }
            }
            yield break;
        }
    }
}
