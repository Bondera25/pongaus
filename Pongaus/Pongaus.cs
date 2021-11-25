using Jypeli;
using Jypeli.Assets;
using Jypeli.Controls;
using Jypeli.Widgets;
using System;
using System.Collections.Generic;


/// <summary>
/// Katri Kettunen 25.11.2021
/// </summary>
public class Pongaus : PhysicsGame
{
    private IntMeter pistelaskuri;
    private DoubleMeter alaspainlaskuri;
    private Timer aikalaskuri;
    private EasyHighScore topLista = new EasyHighScore();


    public override void Begin()
    {
        Alkuvalikko();
        LuoOhjaimet();
        topLista.HighScoreWindow.Closed += delegate { Alkuvalikko(); };
    }


    /// <summary>
    /// Luo pelikentän ja asettaa taustakuvan ja kameran
    /// </summary>
    public void LuoKentta()
    {
        Level.CreateBorders(false);
        Level.Background.Image = LoadImage("tausta");
        Level.Background.ScaleToLevelFull();
        Camera.ZoomToLevel();
    }


    /// <summary>
    /// Luo uuden klikattavan pallon
    /// </summary>
    /// <param name="leveys"></param>
    /// <param name="korkeus"></param>
    /// <param name="x">koordinaatti x</param>
    /// <param name="y">koordinaatti y</param>
    /// <param name="kuva">eläimen kuva</param>
    /// <param name="elinika">sekunteina</param>
    public void LuoPallo(double leveys, double korkeus, double x, double y, string kuva, double elinika)
    {
        GameObject pallo = new GameObject(leveys, korkeus, x, y);
        Add(pallo);
        pallo.Shape = Shape.Circle;
        pallo.Image = LoadImage(kuva);

        //pallo.X = RandomGen.NextDouble(Level.Left, Level.Right);
        //pallo.Y = RandomGen.NextDouble(Level.Top, Level.Bottom);
        pallo.Position = Level.GetRandomFreePosition(leveys);
        pallo.LifetimeLeft = TimeSpan.FromSeconds(elinika);
        pallo.Tag = "eläin";
    }


    /// <summary>
    /// Luo laivan kentälle
    /// </summary>
    /// <param name="leveys"></param>
    /// <param name="korkeus"></param>
    /// <param name="x">koordinaatti x</param>
    /// <param name="y">koordinaatti y</param>
    /// <param name="kuva">laivan kuva</param>
    public void LuoLaiva(double leveys, double korkeus, double x, double y, string kuva)
    {
        PhysicsObject laiva = PhysicsObject.CreateStaticObject(leveys, korkeus);
        Add(laiva);
        laiva.Shape = Shape.Rectangle;
        laiva.Image = LoadImage(kuva);
        laiva.X = x;
        laiva.Y = y;
    }


    /// <summary>
    /// Luo saaren, eli alueen jolle ei luoda palloja
    /// </summary>
    /// <param name="leveys"></param>
    /// <param name="korkeus"></param>
    /// <param name="x">koordinaatti x</param>
    /// <param name="y">koordinaatti y</param>
    /// <param name="kulma">kallistuskulma</param>
    public void LuoSaari(double leveys, double korkeus, double x, double y, double kulma)
    {
        PhysicsObject saari = PhysicsObject.CreateStaticObject(leveys, korkeus);
        Add(saari);
        saari.Shape = Shape.Circle;
        saari.X = x;
        saari.Y = y;
        saari.Angle = Angle.FromDegrees(kulma);
        saari.IsVisible = false;
    }


    /// <summary>
    /// Luodaan pelin ohjaimet 
    /// </summary>
    public void LuoOhjaimet()
    {
        Mouse.Listen(MouseButton.Left, ButtonState.Pressed, Klikkaa, "ota kuva");
        PhoneBackButton.Listen(ConfirmExit, "Lopeta peli");
        Keyboard.Listen(Key.Escape, ButtonState.Pressed, ConfirmExit, "Lopeta peli");
    }


    /// <summary>
    /// Määritellään pistelaskuria kasvattavat ehdot hiirenklikkaukselle, 
    /// tuhoaa klikatun pallon ja kasvattaa pistelaskuria
    /// </summary>
    public void Klikkaa()
    {
        Vector sijainti = Mouse.PositionOnWorld;
        List<GameObject> lista = GetObjectsAt(sijainti, "eläin", 1);
        foreach (GameObject elain in lista)
        {
            elain.Destroy();
            pistelaskuri.Value += 1;
        }
    }


    /// <summary>
    /// Luo pistelaskurin sekä pistenäytön ja määrittelee sen sijainnin ja muut asetukset
    /// </summary>
    private void LuoPistelaskuri()
    {
        pistelaskuri = new IntMeter(0);
        Label pistenaytto = new Label();
        pistenaytto.X = Screen.Left + 100;
        pistenaytto.Y = Screen.Top - 40;
        pistenaytto.TextColor = Color.Black;
        pistenaytto.Color = Color.White;
        pistenaytto.Title = "Pisteet  ";
        pistenaytto.BindTo(pistelaskuri);
        Add(pistenaytto);
    }


    /// <summary>
    /// Luo aikalaskurin ja aikanäytön ja määrittelee niiden toiminnot ja asetukset
    /// </summary>
    private void LuoAikalaskuri()
    {
        alaspainlaskuri = new DoubleMeter(20);
        aikalaskuri = new Timer();
        aikalaskuri.Interval = 0.01;
        aikalaskuri.Timeout += LaskeAlaspain;
        aikalaskuri.Start();

        Label aikanaytto = new Label();
        aikanaytto.X = Screen.Left + 100;
        aikanaytto.Y = Screen.Top - 70;
        aikanaytto.TextColor = Color.Black;
        aikanaytto.Color = Color.White;
        aikanaytto.Title = "Aika   ";
        aikanaytto.DecimalPlaces = 2;
        aikanaytto.BindTo(alaspainlaskuri);
        Add(aikanaytto);
    }


    /// <summary>
    /// Funktion avulla saadaan laskuri laskemaan aikaa alaspäin eli mittaamaan jäljellä olevaa aikaa 
    /// </summary>
    private void LaskeAlaspain()
    {
        alaspainlaskuri.Value -= 0.01;
        if (alaspainlaskuri.Value <= 0)
        {
            AikaLoppui();
        }
    }


    /// <summary>
    /// Luo ajan alaspäinlaskurin ja Aika loppui -näytön ja määrittelee sen asetukset, pysäyttää
    /// aikalaskurin, nollaa laskurit, poistaa klikkaamattomat pallot ja näyttää toplistin
    /// pelin päättyessä. 
    /// </summary>
    private void AikaLoppui()
    {
        Label alaspainlaskuri = new Label(400, 100, "Aika loppui, peli päättyy");
        alaspainlaskuri.X = 0;
        alaspainlaskuri.Y = Screen.Top - 100;
        alaspainlaskuri.TextColor = Color.Red;
        alaspainlaskuri.Color = Color.White;
        Add(alaspainlaskuri);
        aikalaskuri.Stop();
        ClearTimers();
        foreach (GameObject pallo in GetObjectsWithTag("eläin"))
            pallo.Destroy();

        topLista.EnterAndShow(pistelaskuri.Value);
    }


    /// <summary>
    /// AloitaPeli sisältää kaikki pelin aloittamiseen tarvittavat funktiokutsut, muuttujien ja ajastinten alustukset  
    /// </summary>
    public void AloitaPeli()
    {
        ClearAll();//lisätään  aliohjelmakutsut
        LuoKentta();
        LuoLaiva(100, 150, -100, -200, "bonita");
        LuoLaiva(100, 150, -400, -100, "katamaran");
        LuoLaiva(100, 150, 300, 300, "santamaria");
        LuoSaari(600, 300, -120, 100, -17);
        LuoPistelaskuri();
        LuoAikalaskuri();
        LaskeAlaspain();

        LuoOhjaimet();//tämä kutsu viimeiseksi

        Timer.CreateAndStart(0.2, delegate { LuoPallo(40.0, 40.0, -200, -200, "hylje", 4); });
        Timer.CreateAndStart(0.3, delegate { LuoPallo(40.0, 40.0, -200, -200, "delfiini", 3); });
        Timer.CreateAndStart(0.5, delegate { LuoPallo(40.0, 40.0, -200, -200, "valas", 5); });
    }


    /// <summary>
    /// Luodaan pelin aloitusvalikko ja asetetaan sen toiminnot
    /// </summary>
    public void Alkuvalikko()
    {
        MultiSelectWindow alkuvalikko = new MultiSelectWindow("Pongaus-peli Alkuvalikko", "Aloita peli",
                "Parhaat pisteet", "Lopeta");
        Add(alkuvalikko);
        alkuvalikko.AddItemHandler(0, AloitaPeli);
        alkuvalikko.AddItemHandler(1, topLista.Show);
        alkuvalikko.AddItemHandler(2, Exit);
        alkuvalikko.DefaultCancel = 3;
    }
}


