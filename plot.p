set terminal png size 800
set output "grafica.png"
set title "1000 peticiones, 10 peticiones concurrentes"
set size ratio 0.6
set grid y
set xlabel "peticiones"
set ylabel "tiempo de respuesta (ms)"
plot "resultados.csv" using 9 smooth sbezier with lines title "BanUsac"