using System;
using System.Collections.Generic;
using System.Linq;
using Jypeli;
using Jypeli.Assets;
using Jypeli.Controls;
using Jypeli.Effects;
using Jypeli.Widgets;
using Vector3 = System.Numerics.Vector3;

public class Peli : PhysicsGame
{
    Image norsukuva = LoadImage("norsu");
    public override void Begin()
    {
        // Mouse.Listenin sijaan nyt on TouchPanel.Listen.
        // Toistaiseksi vain yhden sormen kuuntelu kerrallaan toimii, eli Gestures-kuuntelijat eivät toimi.
        TouchPanel.Listen(ButtonState.Down, AsetaNorsu, null);

        Device.Accelerometer.Start();
        
    }

    List<Vector3> kallistukset = new List<Vector3>();

    protected override void Update(Time time)
    {
        // Varmistetaan että ollaan saatu uusi arvo.
        // Välillä kiihtyvyysanturi saattaa antaa samaa lukemaan useasti putkeen, joka aiheuttaa helposti vilkkumista.
        Vector3 lukema = Device.Accelerometer.Reading3d;
        if (lukema.Equals(Device.Accelerometer.PreviousReading3d)) return;

        // Ainakin minun puhelimella kiihtyvyysanturin jokaisen mittauksen lukeminen aiheuttaa myös vilkkumista.
        // Pidetään tallessa 60 edellistä lukemaa, ja lasketaan niiden keskiarvo.
        kallistukset.Add(lukema);
        if (kallistukset.Count > 60)
            kallistukset.RemoveAt(0);
        var kallistus = new Vector3(
                                    kallistukset.Average(x => x.X),
                                    kallistukset.Average(x => x.Y),
                                    kallistukset.Average(x => x.Z));

        // Kallistus on väliltä -1..1, joten se pitää muuttaa välille 0..1 väriarvojen takia.
        Level.Background.Color = new Color((kallistus.X + 1) / 2, (kallistus.Y + 1) / 2, (kallistus.Z + 1) / 2);
        base.Update(time);
    }

    private void AsetaNorsu(Touch touch)
    {
        GameObject g = new GameObject(50, 50);
        g.Position = touch.PositionOnWorld;
        g.Image = norsukuva;
        g.LifetimeLeft = TimeSpan.FromSeconds(1);
        Add(g);
    }
}
