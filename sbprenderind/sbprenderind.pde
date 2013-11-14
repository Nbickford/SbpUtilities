PImage blank;
int s=32;
int puzw;
int puzh;
Boolean saveImage=true;
PFont font;

void setup() {
  font=loadFont("Verdana-24.vlw");
  textFont(font);
  textAlign(CENTER);
  
  puzw=6;
  puzh=3;
  size(puzw*s+s/2, puzh*s+s/2);
  blank=new PImage(s, s, RGB);
  blank.loadPixels();
  for (int x=0;x<s;x++) {
    for (int y=0;y<s;y++) {
      switch(mod2(x+y, s/4)) {
      case 0:
        blank.pixels[x+s*y]=color(0, 0, 0);
        break;
      case 1:
      case 7:
        blank.pixels[x+s*y]=color(200, 200, 200);
        break;
      default:
        blank.pixels[x+s*y]=color(255, 255, 255);
        break;
      }
    }
  }
  smooth();
  fill(255);
}

void draw() {
  background(128);
  int[][] board={{2,-1,3,4,5,6},{9,10,10,10,8,7},{9,0,11,11,11,7}};
  translate(s/4,s/4);
  DrawBoard(board,puzw,puzh);
  save("better6x3-2.gif");
  noLoop();
}
int total(String[] ar){
  int t=0;
  for(int i=0;i<ar.length;i++){
    t+=Integer.parseInt(ar[i]);
  }
  return t;
}

int getlast(String str, char c) {
  int p=-1;
  int sL=str.length();
  for (int i=0;i<sL;i++) {
    if (str.charAt(i)==c) {
      p=i;
    }
  }
  return p;
}

void DrawBoard(int[][] ar, int w, int h) {
  for (int y=0;y<h;y++) {
    for (int x=0;x<w;x++) {
      if (ar[y][x]==0) {
        image(blank, s*x, s*y);
      }else if (ar[y][x]==-1){
        noStroke();
        rect(s*x, s*y, s, s);
        fill(0);
        text("A",s*x+s/2,s*y+0.8*s);
        fill(255);
      }
      else {
        noStroke();
        rect(s*x, s*y, s, s);
      }
      stroke(0);
      if (x!=0) {
        if (ar[y][x-1]!=ar[y][x]) {
          line(s*x, s*y, s*x, s*(y+1));
        }
      }
      if (x!=w-1) {
        if (ar[y][x+1]!=ar[y][x]) {
          line(s*(x+1), s*y, s*(x+1), s*(y+1));
        }
      }
      if (y!=0) {
        if (ar[y-1][x]!=ar[y][x]) {
          line(s*x, s*y, s*(x+1), s*y);
        }
      }
      if (y!=h-1) {
        if (ar[y+1][x]!=ar[y][x]) {
          line(s*x, s*(y+1), s*(x+1), s*(y+1));
        }
      }
    }
  }
}

int[][] ParseString(String str, int w, int h) {
  String[] lis=split(str, ' ');
  int[][] res=new int[h][];
  for (int y=0;y<h;y++) {
    res[y]=new int[w];
    for (int x=0;x<w;x++) {
      res[y][x]=Integer.parseInt(lis[x+w*y]);
    }
  }
  return res;
}

int mod2(int a, int m) {
  int t=a%m;
  if (t<0) {
    t+=a;
  }
  return t;
}

