using System;
using System.Threading;

namespace UOStudio.Core.Ultima
{
    public abstract class WorldItem : WorldBlock
    {
        protected bool _selected;
        protected bool _locked;
        protected WorldBlock _owner;
        protected ushort _tileId;
        protected ushort _x;
        protected ushort _y;
        protected sbyte _z;

        protected WorldItem(WorldBlock owner)
        {
            _owner = owner ?? throw new ArgumentNullException(nameof(owner));
        }

        public bool CanBeEdited { get; set; }
        public int Priority { get; set; }
        public short PriorityBonus { get; set; }
        public int PrioritySolver { get; set; }

        public ushort RawTileId => _tileId;
        public short RawZ => _z;

        public WorldBlock Owner
        {
            get => _owner;
            set
            {
                if (_owner != value)
                {
                    _owner.Changed = true;
                    if (_locked)
                    {
                        _owner.RemoveRef();
                    }

                    if (_selected)
                    {
                        _owner.RemoveRef();
                    }

                    _owner = value;

                    if (_owner != null)
                    {
                        _owner.Changed = true;
                        if (_locked)
                        {
                            _owner.AddRef();
                        }

                        if (_selected)
                        {
                            _owner.AddRef();
                        }
                    }
                }
            }
        }

        public ushort TileId
        {
            get => _tileId;
            set
            {
                if (_tileId == value)
                {
                    return;
                }

                _tileId = value;
                DoChanged();
            }
        }

        public ushort X
        {
            get => _x;
            set
            {
                if (_x == value)
                {
                    return;
                }

                _x = value;
                DoChanged();
            }
        }

        public ushort Y
        {
            get => _y;
            set
            {
                if (_y == value)
                {
                    return;
                }

                _y = value;
                DoChanged();
            }
        }

        public sbyte Z
        {
            get => _z;
            set => SetZ(value);
        }

        public bool Selected
        {
            get => _selected;
            set => SetSelected(value);
        }

        public bool Locked
        {
            get => _locked;
            set => SetLocked(value);
        }

        public void Delete()
        {
            SetSelected(false);
            SetLocked(false);
            DoChanged();
        }

        public void UpdatePosition(ushort x, ushort y, sbyte z)
        {
            _x = x;
            _y = y;
            _z = z;
            DoChanged();
        }

        protected void SetZ(sbyte value)
        {
            if (_z == value)
            {
                return;
            }

            _z = value;
            DoChanged();
        }

        private void SetSelected(bool selected)
        {
            if (_owner != null && _selected != selected)
            {
                if (selected)
                {
                    _owner.AddRef();
                }
                else
                {
                    _owner.RemoveRef();
                }
            }

            _selected = selected;
        }

        private void SetLocked(bool locked)
        {
            if (_locked != locked)
            {
                _locked = locked;
                if (_locked)
                {
                    _owner?.AddRef();
                }
                else
                {
                    _owner?.RemoveRef();
                }
            }
        }

        protected void DoChanged()
        {
            if (_owner != null)
            {
                _owner.Changed = true;
            }
        }
    }
}
