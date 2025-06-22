# Rollwagen mit zwei Druckfedern

Done by KPP (pern) in 2025!

For Physics Engines @ ZHAW.

Der Rollwagen hat zwei Druckfedern und stösst gegen zwei Endpuffer. Je nach bumperMode ist der linke Bumper 
fixiert oder frei, wobei man auch einstellen kann, ob der Bumper reibungsfrei oder mit viskoser Reibung ist.

Die Parameter für die Masse, Anfangsgeschwindigkeit, Federlänge und -konstante sind heikel, verstellt man nur 
einen Parameter zu viel, kracht der Wagen in die Puffer. Bei manchen Einstellungen ist die Integration zu ungenau, 
dann kann man mit den Solver Parametern spielen. 

Grundsätzlich stellt man alles im Objekt Car ein oder im gleichnamigen Skript. Dort sind auch alle Kräfte 
definiert, also auch die Kräfte auf den Bumper. Der Exporter soll helfen, die Daten zu exportieren. Diese Klasse 
ist so geschrieben, dass sie möglichst universell ist, damit man sie auch in anderen Projekten einfach einsetzen 
kann.


Um das Auto während der Videoaufnahme zu starten, setzen Sie den Recording-Flag im Car-Skript auf „true“.
