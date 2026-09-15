using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;

namespace TextureGenerator
{
    class Program
    {
        static void Main(string[] args)
        {
            // Determine the output path (Content folder of the main project)
            string contentPath = @"C:\Users\conno\Documents\GitHub\Codes\HomeDraw\HomeDraw\Content";
            
            // Or use a direct path for your specific setup
            // string contentPath = @"C:\Users\conno\Documents\GitHub\Codes\HomeDraw\HomeDraw\Content";
            
            Directory.CreateDirectory(contentPath);
            
            Console.WriteLine($"Generating textures in: {contentPath}");
            
            // Generate house texture
            GenerateHouseTexture(Path.Combine(contentPath, "house.png"));
            Console.WriteLine("Created house.png");
            
            // Generate roof texture
            GenerateRoofTexture(Path.Combine(contentPath, "roof.png"));
            Console.WriteLine("Created roof.png");
            
            // Generate door texture
            GenerateDoorTexture(Path.Combine(contentPath, "door.png"));
            Console.WriteLine("Created door.png");
            
            Console.WriteLine("All textures generated successfully!");
        }
        
        static void GenerateHouseTexture(string filePath)
        {
            // House body: 200x150 pixels
            using (Bitmap bmp = new Bitmap(200, 150))
            using (Graphics g = Graphics.FromImage(bmp))
            {
                // Fill with light brown/tan color
                g.Clear(Color.FromArgb(210, 180, 140));
                
                // Add some detail - window
                using (SolidBrush windowBrush = new SolidBrush(Color.FromArgb(135, 206, 235))) // Light blue
                {
                    g.FillRectangle(windowBrush, 40, 40, 40, 40); // Left window
                    g.FillRectangle(windowBrush, 120, 40, 40, 40); // Right window
                }
                
                // Window frames
                using (Pen framePen = new Pen(Color.FromArgb(101, 67, 33), 3))
                {
                    g.DrawRectangle(framePen, 40, 40, 40, 40);
                    g.DrawRectangle(framePen, 120, 40, 40, 40);
                    
                    // Window cross bars
                    g.DrawLine(framePen, 60, 40, 60, 80);  // Vertical left
                    g.DrawLine(framePen, 40, 60, 80, 60);  // Horizontal left
                    g.DrawLine(framePen, 140, 40, 140, 80); // Vertical right
                    g.DrawLine(framePen, 120, 60, 160, 60); // Horizontal right
                }
                
                bmp.Save(filePath, ImageFormat.Png);
            }
        }
        
        static void GenerateRoofTexture(string filePath)
        {
            // Roof: 250x150 pixels (triangle shape)
            using (Bitmap bmp = new Bitmap(250, 150))
            using (Graphics g = Graphics.FromImage(bmp))
            {
                // Start with transparent background
                g.Clear(Color.Transparent);
                
                // Draw triangle roof
                using (SolidBrush roofBrush = new SolidBrush(Color.FromArgb(178, 34, 34))) // Red
                {
                    Point[] trianglePoints = new Point[]
                    {
                        new Point(0, 150),      // Bottom-left
                        new Point(125, 0),      // Top-center
                        new Point(250, 150)     // Bottom-right
                    };
                    g.FillPolygon(roofBrush, trianglePoints);
                }
                
                // Add roof tiles pattern
                using (Pen tilePen = new Pen(Color.FromArgb(139, 0, 0), 2))
                {
                    for (int y = 30; y < 150; y += 30)
                    {
                        g.DrawLine(tilePen, 25, y, 225, y);
                    }
                    
                    // Vertical lines for tiles
                    for (int x = 50; x < 250; x += 50)
                    {
                        g.DrawLine(tilePen, x, 150 - (x * 150 / 250), x, 150);
                    }
                }
                
                bmp.Save(filePath, ImageFormat.Png);
            }
        }
        
        static void GenerateDoorTexture(string filePath)
        {
            // Door: 60x90 pixels
            using (Bitmap bmp = new Bitmap(60, 90))
            using (Graphics g = Graphics.FromImage(bmp))
            {
                // Fill with dark brown
                g.Clear(Color.FromArgb(101, 67, 33));
                
                // Add door panels
                using (Pen panelPen = new Pen(Color.FromArgb(139, 90, 43), 2))
                {
                    // Top panel
                    g.DrawRectangle(panelPen, 10, 10, 40, 30);
                    
                    // Bottom panel
                    g.DrawRectangle(panelPen, 10, 50, 40, 25);
                }
                
                // Add door knob
                using (SolidBrush knobBrush = new SolidBrush(Color.Gold))
                {
                    g.FillEllipse(knobBrush, 42, 45, 8, 8);
                }
                
                bmp.Save(filePath, ImageFormat.Png);
            }
        }
    }
}
