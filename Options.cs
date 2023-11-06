using Menu.Remix.MixedUI;
using UnityEngine;

namespace Pearlventory;

//from https://github.com/Dual-Iron/osha-compliant-shelters/blob/master/src/Options.cs
sealed class PearlventoryOptions : OptionInterface
{
    public static Configurable<int> MaxPearls;

    public PearlventoryOptions()
    {
        MaxPearls = config.Bind("cfgPearlLimit", 10, new ConfigAcceptableRange<int>(1, 100));
    }

    //OpCheckBox holdDown;
    //OpCheckBox coopHoldDown;

    public override void Initialize()
    {
        base.Initialize();

        Tabs = new OpTab[] { new OpTab(this) };

        var author = new OpLabel(20, 600 - 40, "by Nuclear Pasta", true);
        var github = new OpLabel(20, 600 - 40 - 40, "https://github.com/voidwyrm-2/Pearlventory");

        float y = 340;

        var a = new OpLabel(new(20, y), Vector2.zero, "The storage can hold this many pearls", FLabelAlignment.Left);
        var a2 = new OpSlider(MaxPearls, new Vector2(24, y - 48), 300, vertical: false);

        Tabs[0].AddItems(author, github, a, a2);
    }

    /*
    public override void Update()
    {
        base.Update();

        if (holdDown != null)
        {
            bool greyed = holdDown.value != "true";

            coopHoldDown.greyedOut = greyed;
        }
    }
    */
} 