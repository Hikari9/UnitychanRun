import java.util.*;
import java.io.*;
import java.util.concurrent.*;
import java.awt.*;
import javax.swing.*;

public class TimeGraph {
    
    public static int WIDTH = 1000;
    public static int HEIGHT = 680;
    
    public static Color transparency(Color orig, double alpha) {
        return new Color(orig.getRed(), orig.getGreen(), orig.getBlue(), (int) (orig.getAlpha() * alpha));
    }
    
    public static RenderingHints AntiAliasing;
    static {
        // anti-alias text
        AntiAliasing = new RenderingHints(
            RenderingHints.KEY_TEXT_ANTIALIASING,
			RenderingHints.VALUE_TEXT_ANTIALIAS_ON);
        // anti-alias quality of images
		AntiAliasing.put(
			RenderingHints.KEY_RENDERING,
			RenderingHints.VALUE_RENDER_QUALITY);
        // anti-alias edges
		AntiAliasing.put(
			RenderingHints.KEY_ANTIALIASING,
			RenderingHints.VALUE_ANTIALIAS_ON);
    }
    
    public Color color = Color.GREEN;
    final ConcurrentLinkedQueue<Pair> values = new ConcurrentLinkedQueue<Pair>();
    final long keep; // care only about 10 seconds within time
    final double minY, maxY;
    
    public void put(double val) {
        long time = System.currentTimeMillis();
        values.offer(new Pair(time, val));
    }
    
    public TimeGraph(final long keep, double minY, double maxY) {
        this.keep = keep;
        this.minY = minY;
        this.maxY = maxY;
        new java.util.Timer(true).scheduleAtFixedRate(
            new TimerTask() {
                public void run() {
                    long time = System.currentTimeMillis();
                    while (!values.isEmpty() && values.peek().x < time - keep)
                        values.poll();
                }
            }, 0, 5);
    }
    
    public TimeGraph() {
        this(5000, -400, 400);
    }
    
    public Polygon getPolygon() {
      // get polygon
        long time = System.currentTimeMillis();
        long show = keep + 2000;
        // show graph until keep + 2000
        
        ArrayList<Point> list = new ArrayList<Point>();
        list.ensureCapacity(values.size() + 2);
        
        int firstX = -1;
        int lastX = 0;
        
        for (Pair p : values) {
            double scaleX, scaleY;
            scaleX = ((double) (p.x - (time - keep))) / (show);
            scaleY = ((double) (p.y - minY)) / (maxY - minY);
            if (scaleX < 0 || scaleX > 1)
              continue;
            scaleY = 1 - scaleY; // flip
            int x = (int) (scaleX * WIDTH);
            int y = (int) (scaleY * HEIGHT);
            list.add(new Point(x, y));
            if (firstX == -1) firstX = x;  
            lastX = x;
        }
                
        list.add(new Point(lastX, HEIGHT / 2));
        list.add(new Point(firstX, HEIGHT / 2));
        
        int[] x = new int[list.size()];
        int[] y = new int[list.size()];
        
        for (int i = 0; i < list.size(); ++i) {
            x[i] = list.get(i).x;
            y[i] = list.get(i).y;
        }
        
        Polygon p = new Polygon(x, y, x.length);
        return p;
    }
    
    public void draw(Graphics2D g) {
        Polygon p = getPolygon();
        
        g.setColor(transparency(color, 0.5));
        g.fillPolygon(p);
        
        g.setColor(color);
        g.drawPolygon(p);
        
        int ly = (p.ypoints[p.npoints - 3]);
        g.drawLine(0, ly, WIDTH, ly);
    }
    
    public void setColor(Color c) {
      color = c;
    }
    
    public ConcurrentLinkedQueue getValues() {
      return values;
    }
    
}

class TimeCanvas extends JComponent {
    
    public static int FPS = 24;
    
    public final TimeGraph[] graphs;
    public TimeCanvas(TimeGraph[] graphs) {
        this.graphs = graphs;
        new java.util.Timer(true).scheduleAtFixedRate(
            new TimerTask() {
                public void run() {
                    repaint();
                }
            }, 0, 1000 / FPS
        );
    }
    @Override
    public void paintComponent(Graphics g) {
        super.paintComponent(g);
        Graphics2D g2 = (Graphics2D) g;
        
        g2.setRenderingHints(TimeGraph.AntiAliasing);
        g2.scale((double) getWidth() / TimeGraph.WIDTH, (double) getHeight() / TimeGraph.HEIGHT);
        for (TimeGraph tg : graphs) {
            tg.draw(g2);
        }
    }
}

class Pair {
    public long x;
    public double y;
    public Pair(long x, double y) {
        this.x = x;
        this.y = y;
    }
    public String toString() {
      return "(" + x + ", " + y + ")";
    }
}
