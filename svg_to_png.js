// TODO
let mySVG    = document.querySelector('…');      // Inline SVG element
let tgtImage = document.querySelector('…');      // Where to draw the result
let can      = document.createElement('canvas'); // Not shown on page
let ctx      = can.getContext('2d');
let loader   = new Image;                        // Not shown on page

loader.width  = can.width  = tgtImage.width;
loader.height = can.height = tgtImage.height;
loader.onload = function(){
  ctx.drawImage( loader, 0, 0, loader.width, loader.height );
  tgtImage.src = can.toDataURL();
};
let svgAsXML = (new XMLSerializer).serializeToString( mySVG );
loader.src = 'data:image/svg+xml,' + encodeURIComponent( svgAsXML );
