namespace AndroPen.Controls;
internal class ProgressBar : Control
{
    private float _progress = 0f;
    internal float Progress
    {
        get => this._progress;
        set
        {
            if( value < 0f )
                this._progress = 0f;
            else if ( value > 1f )
                this._progress = 1f;
            else
                this._progress = value;
            Invalidate();
        }
    }
    protected override void OnPaint( PaintEventArgs e )
    {
        //base.OnPaint( e );
        e.Graphics.Clear(this.BackColor);
        e.Graphics.FillRectangle( new SolidBrush( this.ForeColor ), new(
            2, 2,
            (int)( ( this.Width - 4 ) * this._progress ),
            this.Height - 4 ) );
    }
}
