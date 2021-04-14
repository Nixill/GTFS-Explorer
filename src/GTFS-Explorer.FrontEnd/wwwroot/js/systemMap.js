const systemMap = L.map('system-map').setView([51.5, -0.09], 13);
const osmAttribution = '&copy; <a href="https://www.openstreetmap.org/copyright">OpenStreetMap</a> contributors';
const osmTileUrl = 'https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png';

const tiles = L.tileLayer(osmTileUrl, {
    attribution: osmAttribution
});

tiles.addTo(systemMap);
    
const marker = L.marker([51.5, -0.09])

marker.addTo(systemMap)
    .bindPopup('A pretty CSS3 popup.<br> Easily customizable.')
    .openPopup();