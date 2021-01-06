using System;
using System.Collections.Generic;
using Jypeli;
using Jypeli.Widgets;
/// @author Tatja Syrjamaki
/// @version 22.11.2020
/// <summary>
/// Seikkailupeli, jossa paetaan haita ja varotaan esteita.
/// </summary>
public class HuiHai : PhysicsGame
{

    private static readonly Image hainKuva = LoadImage("hai24.png");
    private static readonly Image timKuva = LoadImage("tim.png");
    private static readonly Image korallitKuva = LoadImage("korallit.png");
    private static readonly Image meri2Kuva = LoadImage("meri2.png");
    private static readonly Image mustekalaKuva = LoadImage("mustekala2.png");
    private static readonly Image levaKuva = LoadImage("leva.png");
    private static readonly Image merihevonenKuva = LoadImage("merihevonen.png");


    private Timer aikaLaskuri = new Timer();
    private EasyHighScore topLista = new EasyHighScore();
    private List<PhysicsObject> liikutettavat = new List<PhysicsObject>();
    private const int vaikeusaste = 4;
    private int lisaPisteet = 0;
    private const int VAIKEUSKIIHDYTIN = 4;
    private const double LIIKKUMISNOPEUS = 300;
    private PhysicsObject hai;
    private PhysicsObject tim;


    /// <summary>
    /// Hain ja timin sijainti pelissa, ajan paivittaminen
    /// </summary>
    protected override void Update(Time time)
    {
        base.Update(time);
        hai.Y = tim.Y;
    }


    /// <summary>
    /// Aloittaa pelin
    /// </summary>
    public override void Begin()
    {
        AloitaPeli();
    }


    /// <summary>
    /// aloittaa pelin ajan laskun ja nollaa lisapisteet, maarittaa hain ja pelaajan ominaisuudet, ajastaa uhkien luontia ja liikkumista
    /// </summary>
    public void AloitaPeli()
    {   
        aikaLaskuri.Start();
        aikaLaskuri.Reset();
        lisaPisteet = 0;
        Label aikaNaytto = new Label();
        aikaNaytto.TextColor = Color.White;
        aikaNaytto.DecimalPlaces = 0;
        aikaNaytto.BindTo(aikaLaskuri.SecondCounter);
        aikaNaytto.X = -400;
        aikaNaytto.Y = 314;
        Add(aikaNaytto);

        LuoMaailma();

        //Hain ominaisuudet & tormaystapahtumas
        hai = new PhysicsObject(250, 180);
        hai.Shape = Shape.Rectangle;
        hai.Mass = 10.0;
        hai.Image = hainKuva;
        Add(hai);
        hai.X = -300.0;
        hai.Y = 400.0;
        hai.CanRotate = false;
        hai.CollisionIgnoreGroup = 1;
        hai.Tag = "hai";

        // Luodaan pelaaja tim kala & ominaisuudet
        tim = new PhysicsObject(200, 300);
        tim.Shape = Shape.Rectangle;
        tim.Mass = 10.0;
        tim.Image = timKuva;
        Add(tim);
        tim.Size = new Vector(120, 120);
        tim.X = -20.0;
        tim.Y = -100.0;
        tim.CanRotate = false;
        AddCollisionHandler(tim, "hai", PelaajaOsuuHai);

        //Ajastin uhkien luomiseen
        Timer uhkaAjastin = new Timer();
        uhkaAjastin.Interval = 1;
        uhkaAjastin.Timeout += arvoUhka;
        uhkaAjastin.Start();

        // Ajastin liikuttamiseen
        Timer ajastin = new Timer();
        ajastin.Interval = 0.05;
        ajastin.Timeout += LiikutaUhkaa;
        ajastin.Start();

        AsetaNappaimet();
    }


    /// <summary>
    /// Liikuttaa esteita ja kiihdyttaa pelin aikaa ja vaikeutta
    /// </summary>
    private void LiikutaUhkaa()
    {
        for (int i = 0; i < liikutettavat.Count; i++)
        {
            PhysicsObject uhka = liikutettavat[i];
            uhka.Velocity = new Vector(-100 - aikaLaskuri.SecondCounter.Value * VAIKEUSKIIHDYTIN, 0);
        }


    }


    /// <summary>
    /// Generoi esteita satunnaisesti pitaa ylla lisapisteita
    /// </summary>
    private void arvoUhka()
    {
        int luodaankoUhkaArpoja = RandomGen.NextInt(0, 10);
        if (luodaankoUhkaArpoja >= vaikeusaste)
        {
            lisaPisteet += luoUhka(RandomGen.NextInt(0, 5));
        }

    }


    /// <summary>
    /// valitun esteen generointi ja lisapisteen palautus funktio, maarittaa esteiden ominaisuudet
    /// </summary>
    /// <param name="uhkaArpoja">Mikä uhka luodaan</param>
    /// <returns>Esteista saatupiste</returns>
    private int luoUhka(int uhkaArpoja)//
    {
        int uhanPisteet;
        PhysicsObject uhka = new PhysicsObject(111, 122);
        switch (uhkaArpoja)
        {
            /// <summary>
            /// Mustekala esteen omninaisuudet ja lisapisteen maaritys
            /// </summary>
            case 1: 
                uhka.Width = 500;
                uhka.Height = 200;
                uhka.Image = mustekalaKuva;
                uhka.Y = RandomGen.NextInt(-200, 300);
                uhanPisteet = 5;
                break;
            /// <summary>
            /// Merihevos esteen ominaisuudet ja lisapisteen maaritys
            /// </summary>
            case 2:  
                uhka.Width = 300;
                uhka.Height = 200;
                uhka.Image = merihevonenKuva;
                uhka.Y = RandomGen.NextInt(-100, 300);
                uhanPisteet = 2;
                break;
            /// <summary>
            /// Leva esteen ominaisuudet ja lisapisteen maaritys
            /// </summary>
            case 3: 
                uhka.Width = 300;
                uhka.Height = 200;
                uhka.Image = levaKuva;
                uhka.Y = RandomGen.NextInt(-100, 300);
                uhka.Y = (uhka.Height) / 2 - 400;
                uhanPisteet = 2;
                break;
            /// <summary>
            /// Koralli esteen ominaisuudet ja lisapisteen maaritys
            /// </summary>
            case 4:
                uhka.Image = korallitKuva;
                uhka.Width = 400;
                uhka.Height = RandomGen.NextInt(100, 400);
                uhka.Y = (uhka.Height) / 2 - 400;
                uhanPisteet = 3;
                break;
            default:
                return 0;
        }


        //Esteiden ominaisuudet
        uhka.CanRotate = false;
        uhka.Shape = Shape.Circle;
        uhka.CollisionIgnoreGroup = 1;
        uhka.Mass = 1000.0;
        uhka.X = Level.Right;


        uhka.Tag = "este";//este
        Add(uhka);
        liikutettavat.Add(uhka);
        return uhanPisteet;
    }


    /// <summary>
    /// Maarittaa taustan ja reunojen ominaisuudet, tuhoaa oikean ja vasemman reunan, zoomaa kameran
    /// </summary>
    private void LuoMaailma()
    {
        Gravity = new Vector(0, -10);
        Level.Background.Image = meri2Kuva;
        Surfaces reunat = Level.CreateBorders(false);
        reunat.Left.Destroy();
        reunat.Right.Destroy();
        Camera.ZoomToLevel();
    }


    /// <summary>
    /// Maarittaa pelin nappaimet ja liikkumisnopeuden
    /// </summary>
    private void AsetaNappaimet()
    {
        Keyboard.Listen(Key.Down, ButtonState.Down, Liikuta, null, tim, new Vector(0, -LIIKKUMISNOPEUS));
        Keyboard.Listen(Key.Down, ButtonState.Released, Liikuta, null, tim, Vector.Zero);
        Keyboard.Listen(Key.Up, ButtonState.Down, Liikuta, null, tim, new Vector(0, LIIKKUMISNOPEUS));
        Keyboard.Listen(Key.Up, ButtonState.Released, Liikuta, null, tim, Vector.Zero);

        PhoneBackButton.Listen(ConfirmExit, "Lopeta peli");
        Keyboard.Listen(Key.Escape, ButtonState.Pressed, ConfirmExit, "Lopeta peli");
    }


    /// <summary>
    /// Tuhoaa pelaajan, luo high scorelistan ja lisaa lisapisteet, aloittaa uuden pelin valilyönnilla
    /// </summary>
    void PelaajaOsuuHai(PhysicsObject pelaaja, PhysicsObject kohde)
    {
        pelaaja.Destroy();
        topLista.EnterAndShow(Math.Round(aikaLaskuri.SecondCounter.Value)+lisaPisteet);
        Keyboard.Listen(Key.Space, ButtonState.Down, AloitaAlusta, null);
      
    }


    /// <summary>
    /// Alustaa kaikki vanhat muuttujat ja aloittaa pelin alusta 
    /// </summary>
    public void AloitaAlusta()
    {

        ClearAll();
        AloitaPeli();

    }


    /// <summary>
    /// Liikuttaa pelaajaa eli Tim kalaa maaratylla akselilla tietylla nopeudella
    /// </summary>
    void Liikuta(PhysicsObject tim, Vector suunta)
    {
        Vector nopeus = tim.Velocity;
        nopeus.Y = suunta.Y;
        tim.Velocity = nopeus;

    }
}

