# Barycentric

Aplicação interativa em C++ e OpenGL 3.3 para visualização de triângulos 2D, cálculo de coordenadas baricêntricas em Fragment Shader (GLSL) e inspeção visual da distância do cursor até o baricentro do triângulo.

## Funcionalidades

* **Detecção de ponto em triângulo:** cálculo real de coordenadas baricêntricas $(u, v, w)$ na GPU via `Shaders/Image.fs`.
* **Visualização interativa de distância:** ao passar o mouse sobre a área interna do triângulo, o shader renderiza:
  * o ponto do baricentro $\left(\frac{A + B + C}{3}\right)$;
  * o segmento de reta entre o cursor e o centro;
  * um círculo centrado no baricentro com raio igual à distância atual;
  * preenchimento com intensidade proporcional ao afastamento do centro.
* **Monitoramento de desempenho:** atualização em tempo real do FPS no título da janela.

## Controles

| Ação | Controle |
| :--- | :--- |
| Iniciar / pausar renderização | `Espaço` |
| Inspecionar distância ao centro | Passar o mouse sobre o triângulo |
| Alternar flag `inflow` | `I` |
| Fechar aplicação | `ESC` |

## Pré-requisitos (Linux / WSL Ubuntu)

Instale o compilador, o CMake e as bibliotecas do OpenGL/GLFW:

```bash
sudo apt update
sudo apt install -y build-essential cmake libgl1-mesa-dev libx11-dev libxi-dev libglx-mesa0 libglfw3-dev mesa-utils
```

## Como compilar e executar

Execute os comandos sempre a partir da **raiz do projeto** (para que o executável encontre a pasta `Shaders/` corretamente):

```bash
cmake -B build
cmake --build build
./build/Barycentric
```

Ao abrir a janela, pressione **Espaço** para ativar a renderização (caso `shouldDraw` esteja definido como `false` em `Barycentric.cpp`).

> **Nota sobre alterações no shader:** arquivos em `Shaders/` são lidos em tempo de execução. Edições em `Shaders/Image.fs` exigem apenas reiniciar `./build/Barycentric`, sem recompilar o C++.

## Estrutura principal

```text
barycentric/
├── Barycentric.cpp      # Janela GLFW, buffers OpenGL, loop de renderização e input
├── Shader.h             # Leitura, compilação e gerenciamento de uniforms GLSL
├── glad.c               # Loader das funções do OpenGL
├── CMakeLists.txt       # Configuração de build
├── includes/            # Headers locais (GLAD, GLM, etc.)
└── Shaders/
    ├── VertexShader.vs  # Vertex shader principal
    ├── Image.fs         # Fragment shader com coordenadas baricêntricas e interação
    └── common.incl      # Trecho comum anexado aos fragment shaders
```

## Notas para WSL

Ao executar via WSLg no Windows, avisos de `libEGL` ou `MESA-LOADER` / `ZINK` no terminal indicam apenas o fallback automático para renderização por software e não impedem o funcionamento da aplicação.