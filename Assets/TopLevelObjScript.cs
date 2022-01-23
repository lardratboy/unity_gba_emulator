using UnityEngine;
using System.Collections;
using System;
using System.IO;
using System.Threading;
using System.Runtime.InteropServices;

namespace GarboDev
{
    struct Diagnostics
    {
        public static void PRINT(String s)
        {
            MonoBehaviour.print(s);
        }
    }
}

public class LiveButtonState {
	public GameObject source;
}

public class TopLevelObjScript : MonoBehaviour
{
    public TextAsset _romAsset;
    public TextAsset _biosAsset;

    private Texture2D _displayTexture;
    private GarboDev.GbaManager _gbaManager = null;
    //private GarboDev.GBASoundPlayer soundPlayer = null;

    private String _storagePrefix = "";

    private Color[] _displayTable = new Color[ 240 * 160 ];
    private Color[] _colorLookupTable = new Color[65536];

	private Color32[] _displayTable32 = new Color32[ 240 * 160 ];
	private Color32[] _colorLookupTable32 = new Color32[65536];

	public GameObject _button_A;
	public GameObject _button_B;
	public GameObject _button_UP;
	public GameObject _button_DOWN;
	public GameObject _button_LEFT;
	public GameObject _button_RIGHT;
	public GameObject _button_SHOULDER_LEFT;
	public GameObject _button_SHOULDER_RIGHT;
	public GameObject _button_SELECT;
	public GameObject _button_START;

    // Use this for initialization
    void Start()
    {
        // generate the color lookup table

        for (int i = 0; i < 32768; i++)
        {
            _colorLookupTable[i] = new Color(
                (float)(i & 0x001F) / (float)0x001F,
                (float)(i & 0x03E0) / (float)0x03E0,
                (float)(i & 0x7C00) / (float)0x7C00,
                1.0f );

            _colorLookupTable[i+32768] = _colorLookupTable[i];

			_colorLookupTable32[i] = _colorLookupTable[i];
			_colorLookupTable32[i+32768] = _colorLookupTable[i];

        }

        _displayTexture = new Texture2D(240, 160, TextureFormat.ARGB32, false);
        _displayTexture.anisoLevel = 0;
        _displayTexture.filterMode = FilterMode.Point;

        _gbaManager = new GarboDev.GbaManager();

        _gbaManager.KeyState = 0x3FF; // hack

        //soundPlayer = new GarboDev.GBASoundPlayer(gbaManager.AudioMixer, 2);
        //soundPlayer.Resume();
        //soundPlayer.Pause();

        // gbaManager.Halt();

        _gbaManager.VideoManager.Presenter = GBAUpdated;

        GarboDev.Renderer renderer = new GarboDev.Renderer();
        renderer.Initialize(null);

        _gbaManager.VideoManager.Renderer = renderer;

        LoadBios();
		LoadROM( (TextAsset)_romAsset ); //"software-rendering-demo");

        //soundPlayer.Resume();
		//_gbaManager.Reset();
		//_gbaManager.Resume();

	}

    private void LoadBios()
    {
        try
        {

#if true

            TextAsset asset = _biosAsset; // Resources.Load("gba_bios") as TextAsset;

            byte[] rom = new byte[(int)asset.bytes.Length];

            _gbaManager.LoadBios(asset.bytes);

            print("LOADED BIOS!!!!");

#else

            string biosFilename = _storagePrefix + "gba_bios.bytes";
            using (Stream stream = new FileStream(biosFilename, FileMode.Open))
            {
                byte[] rom = new byte[(int)stream.Length];
                stream.Read(rom, 0, (int)stream.Length);

                _gbaManager.LoadBios(rom);

                print("LOADED BIOS!!!!");
            }
#endif

        }
        catch (Exception exception)
        {
            print("Unable to load bios file, disabling bios (irq's will not work)\n" + exception.Message);
        }
    }

	public void LoadROM(TextAsset rom)
	{

#if true
            //TextAsset rom = _romAsset; // Resources.Load(romName) as TextAsset;

            int romSize = 1;
            while (romSize < rom.bytes.Length)
            {
                romSize <<= 1;
            }

            byte[] romBytes = new byte[romSize];

            rom.bytes.CopyTo( romBytes, 0 );

			_gbaManager.Halt();

            _gbaManager.LoadRom(romBytes);

            print("LOADED ROM!!!!");

			_gbaManager.Reset();
			_gbaManager.Resume();

#else

        using (Stream stream = new FileStream(_storagePrefix + romName, FileMode.Open, FileAccess.Read, FileShare.Read))
        {
            int romSize = 1;
            while (romSize < stream.Length)
            {
                romSize <<= 1;
            }

            byte[] rom = new byte[romSize];
            stream.Read(rom, 0, (int)stream.Length);

            _gbaManager.LoadRom(rom);

            print("LOADED ROM!!!!");
        }

#endif

    }

    private int _updateCounter = 0;

    private Stack _updateStack = new Stack();

    public void GBAUpdated(object data)
    {
        uint[] buffer = (uint[])data;

        _updateStack.Push(buffer);

        ++_updateCounter;
    }

	ushort BitsIfActive( GameObject source, ushort bitmask ) {
		if ( null == source ) return 0;
		LiveButton s = source.GetComponent<LiveButton>();
		if ( !s.isActive ) return (ushort)0;
		return bitmask;
	}


    // Update is called once per frame
    void Update()
    {
		/*
        try
        {
            print(RenderTexture.active.width + ", " + RenderTexture.active.height);
        }
        catch
        {
        }
        */

		ushort keystate = 0x3FF;

		/*
		#define P1_BUTTON_A		0x0001
		#define P1_BUTTON_B		0x0002
		#define P1_SELECT		0x0004
		#define P1_START		0x0008
		#define P1_RIGHT		0x0010
		#define P1_LEFT			0x0020
		#define P1_UP			0x0040
		#define P1_DOWN			0x0080
		#define P1_BUTTON_R		0x0100
		#define P1_BUTTON_L		0x0200
		*/

		keystate ^= BitsIfActive( _button_A, (ushort)0x0001 );						
		keystate ^= BitsIfActive( _button_B, (ushort)0x0002 );
		keystate ^= BitsIfActive( _button_SELECT, (ushort)0x0004 );
		keystate ^= BitsIfActive( _button_START, (ushort)0x0008 );
		keystate ^= BitsIfActive( _button_RIGHT, (ushort)0x0010 );
		keystate ^= BitsIfActive( _button_LEFT, (ushort)0x0020 );
		keystate ^= BitsIfActive( _button_UP, (ushort)0x0040 );
		keystate ^= BitsIfActive( _button_DOWN, (ushort)0x0080 );
		keystate ^= BitsIfActive( _button_SHOULDER_RIGHT, (ushort)0x0100 );
		keystate ^= BitsIfActive( _button_SHOULDER_LEFT, (ushort)0x0200 );

		_gbaManager.KeyState = keystate;

        if (0 != _updateStack.Count)
        {
            uint[] b = (uint[])_updateStack.Pop();

            _updateStack.Clear();

            if (_displayTexture)
            {
				// the 32 functions seem to work 25% (maybe more) faster

				if ( true ) {

					for (int i = 0; i < (240 * 160); ++i )
					{
						_displayTable32[i] = _colorLookupTable32[b[i]]; // the real question is can a uint array be converted to Color32 fast?
					}
					
					_displayTexture.SetPixels32( _displayTable32 );

				} else {

	                for (int i = 0; i < (240 * 160); ++i )
	                {
	                    _displayTable[i] = _colorLookupTable[b[i]];
	                }

	                _displayTexture.SetPixels(0, 0, 240, 160, _displayTable, 0);

				}
                _displayTexture.Apply(false);

                this.GetComponent<UnityEngine.Renderer>().material.mainTexture = _displayTexture;
            }

            //Thread.Sleep(5);
        }
    }
}
