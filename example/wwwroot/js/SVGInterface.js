export function addLine(id, x1, y1, x2, y2, style) {
    var svg = document.getElementById("svg");
    var line = document.createElementNS('http://www.w3.org/2000/svg','line');
    line.setAttribute("id", id);
    line.setAttribute("x1", x1);
    line.setAttribute("y1", y1);
    line.setAttribute("x2", x2);
    line.setAttribute("y2", y2);
    line.setAttribute("style", style);
    svg.appendChild(line)
}

export function removeLine(id) {
    var line = document.getElementById(id);
    if (line != null) {
        line.remove();
    }
}

export function clear() {
    var svg = document.getElementById("svg");
    svg.innerHTML = "";
}