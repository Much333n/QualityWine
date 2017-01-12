using System.Linq;
using Microsoft.Xna.Framework;
using StardewModdingAPI;
using StardewModdingAPI.Events;
using StardewValley;

namespace QualityWine
{
    /// <summary>The mod entry point.</summary>
    public class ModEntry : Mod
    {
        /*********
        ** Public methods
        *********/
        /// <summary>The mod entry point, called after the mod is first loaded.</summary>
        /// <param name="helper">Provides simplified APIs for writing mods.</param>
        public override void Entry(IModHelper helper)
        {
            PlayerEvents.InventoryChanged += this.ReceiveInventoryChanged;
        }


        /*********
        ** Public methods
        *********/
        /// <summary>A method invoked when the player's inventory changes.</summary>
        /// <param name="sender">The event sender.</param>
        /// <param name="e">The event arguments.</param>
        private void ReceiveInventoryChanged(object sender, EventArgsInventoryChanged e)
        {
            // validate that only one item was dropped
            {
                bool anyAdded = e.Added.Any();
                bool anyRemoved = e.Removed.Any();
                bool anyChanged = e.QuantityChanged.Any();

                if (anyAdded)
                    return; // not a keg drop
                if (!anyRemoved && !anyChanged)
                    return; // nothing dropped
                if (anyRemoved && anyChanged)
                    return; // multiple items dropped
                if (anyChanged && e.QuantityChanged.Single().StackChange != -1)
                    return; // too many dropped to be a keg drop
            }

            // get dropped item
            var item = (e.Removed.SingleOrDefault()?.Item ?? e.QuantityChanged.Single().Item) as Object;
            if (item == null)
                return; // not a relevant item type
            if (item.category != -75 && item.category != -79)
                return; // not a relevant type

            // get the target keg
            Vector2 toolPos = Game1.player.GetToolLocation();
            Object keg = Game1.currentLocation.getObjectAt((int)toolPos.X, (int)toolPos.Y) as Object;
            if (keg == null || keg.heldObject == null)
                return;

            // transfer quality
            if (keg.Name == "Keg" || keg.Name == "Preserves Jar")
                    if (keg.heldObject.quality == Object.lowQuality)
                        keg.heldObject.quality = item.quality;
        }
    }
}
