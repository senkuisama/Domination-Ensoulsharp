using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EnsoulSharp;
using EnsoulSharp.SDK;

namespace e.Motion_Gangplank
{
    class DelayManager
    {
        private Spell _spellToUse;
        private int _expireTime;
        private int _lastuse;
        private AIBaseClient target;
        
        public DelayManager(Spell spell, int expireTicks)
        {
            _spellToUse = spell;
            _expireTime = expireTicks;
        }

        public void Delay(AIBaseClient enemy)
        {
            _lastuse = Environment.TickCount;
            target = enemy;
        }

        public bool Active()
        {
            return (target != null && _lastuse + _expireTime >= Environment.TickCount);
        }

        public void CheckEachTick()
        {
            if (target != null && _lastuse + _expireTime >= Environment.TickCount && _spellToUse.IsReady() && _spellToUse.Range >= Program.Player.Distance(target))
            {
                _spellToUse.Cast(target);
                target = null;
                //Chat.Print("Casted with DelayManager(TM)");
            }
        }
    }
}
