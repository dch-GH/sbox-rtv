using Editor;
using Sandbox;
using Sandbox.Diagnostics;

namespace RTV;

[Dock( "Editor", "Runtime Texture Viewer", "snippet_folder" )]
public class RuntimeTextureViewer : Widget
{
	private ContextMenu _menu;
	private Pixmap? _pix;

	//private bool _preserveAspectRatio;

	public RuntimeTextureViewer( Widget parent ) : base( parent )
	{
		// Layout top to bottom
		SetLayout( LayoutMode.TopToBottom );

		// Fill the top
		Layout.AddStretchCell();

		Utility.OnInspect += Utility_OnInspect;
	}

	public override void Close()
	{
		base.Close();
		Utility.OnInspect -= Utility_OnInspect;
	}

	protected override void OnClosed()
	{
		base.OnClosed();
		Utility.OnInspect -= Utility_OnInspect;
	}

	protected override void OnContextMenu( ContextMenuEvent e )
	{
		base.OnContextMenu( e );
		_menu = new();
		_menu.AddOption( "Clear", "delete", Clear );
		//_menu.AddOption( "Preserve Aspect", "photo", () => { _preserveAspectRatio = !_preserveAspectRatio; } );
		_menu.OpenAtCursor();
	}

	protected override void OnPaint()
	{
		base.OnPaint();
		Paint.ClearPen();
		Paint.SetBrushEmpty();
		Paint.DrawRect( LocalRect );
		if ( _pix is null )
			return;

		Paint.Draw( LocalRect, _pix );
	}

	private bool TryLoadTexture( Texture texture )
	{
		Assert.True( texture.Width > 0 && texture.Height > 0, "Texture width/height invalid!" );
		var pixels = texture.GetPixels();
		if ( pixels.Length <= 0 || texture.Size.x <= 0 || texture.Size.y <= 0 )
		{
			Log.Error( $"Texture has no data! :: {texture.ResourceName} :: {texture}" );
			return false;
		}

		_pix = new Pixmap( texture.Size );
		var bgraPixels = new byte[pixels.Length * 4];
		for ( int i = 0; i < pixels.Length; i++ )
		{
			bgraPixels[i * 4] = pixels[i].b;
			bgraPixels[i * 4 + 2] = pixels[i].r;
			bgraPixels[i * 4 + 1] = pixels[i].g;
			bgraPixels[i * 4 + 3] = pixels[i].a;
		}

		return _pix.UpdateFromPixels( bgraPixels, texture.Size, texture.ImageFormat );
	}

	private void Clear()
	{
		_pix = null;
	}

	private void Utility_OnInspect( object x )
	{
		if ( x is not Texture texture )
			return;

		TryLoadTexture( texture );
	}
}
