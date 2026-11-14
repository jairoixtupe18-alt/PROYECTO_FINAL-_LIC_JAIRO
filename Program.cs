#nullable enable

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace MarketDonJuan
{
    // ======================================================
    // CLASE 1: PRODUCTO
    // Guarda la información de los productos de la tienda.
    // ======================================================

    public class Producto
    {
        public string Codigo { get; }
        public string Nombre { get; private set; }
        public string Categoria { get; private set; }
        public double Precio { get; private set; }
        public int Stock { get; private set; }

        // Constructor para crear un producto nuevo.
        public Producto(
            string codigo,
            string nombre,
            string categoria,
            double precio,
            int stock)
        {
            if (string.IsNullOrWhiteSpace(codigo))
            {
                throw new ArgumentException(
                    "El código del producto es obligatorio.");
            }

            RevisarDatos(nombre, categoria, precio, stock);

            Codigo = codigo.Trim();
            Nombre = nombre.Trim();
            Categoria = categoria.Trim();
            Precio = precio;
            Stock = stock;
        }

        // Cambia los datos sin modificar el código.
        public void CambiarDatos(
            string nombre,
            string categoria,
            double precio,
            int stock)
        {
            RevisarDatos(nombre, categoria, precio, stock);

            Nombre = nombre.Trim();
            Categoria = categoria.Trim();
            Precio = precio;
            Stock = stock;
        }

        // Comprueba que la información sea válida.
        private static void RevisarDatos(
            string nombre,
            string categoria,
            double precio,
            int stock)
        {
            if (string.IsNullOrWhiteSpace(nombre))
            {
                throw new ArgumentException(
                    "Debe ingresar el nombre del producto.");
            }

            if (string.IsNullOrWhiteSpace(categoria))
            {
                throw new ArgumentException(
                    "Debe ingresar la categoría.");
            }

            if (double.IsNaN(precio) ||
                double.IsInfinity(precio) ||
                precio <= 0)
            {
                throw new ArgumentException(
                    "El precio debe ser mayor que cero.");
            }

            if (stock < 0)
            {
                throw new ArgumentException(
                    "La cantidad no puede ser negativa.");
            }
        }
    }


    // ======================================================
    // CLASE 2: NODO AVL
    // Cada nodo guarda un producto y sus conexiones.
    // ======================================================

    public class NodoAVL
    {
        public Producto Producto { get; }

        public NodoAVL? Izquierda { get; internal set; }

        public NodoAVL? Derecha { get; internal set; }

        public int Altura { get; internal set; }

        public NodoAVL(Producto producto)
        {
            Producto = producto ??
                throw new ArgumentNullException(nameof(producto));

            Izquierda = null;
            Derecha = null;

            // Un nodo nuevo comienza con altura 1.
            Altura = 1;
        }
    }


    // ======================================================
    // CLASE 3: ARBOL AVL
    //
    // Organiza los productos utilizando sus códigos.
    // Se mantiene equilibrado mediante rotaciones.
    // ======================================================

    public class ArbolAVL
    {
        private NodoAVL? raiz;

        private int cantidadProductos;

        // Comparamos los códigos sin distinguir mayúsculas.
        private static readonly StringComparer comparador =
            StringComparer.OrdinalIgnoreCase;

        public int CantidadProductos
        {
            get { return cantidadProductos; }
        }

        public ArbolAVL()
        {
            raiz = null;
            cantidadProductos = 0;
        }


        // --------------------------------------------------
        // OBTENER ALTURA
        // --------------------------------------------------

        private static int ObtenerAltura(NodoAVL? nodo)
        {
            if (nodo == null)
                return 0;

            return nodo.Altura;
        }


        // --------------------------------------------------
        // ACTUALIZAR ALTURA
        // --------------------------------------------------

        private static void ActualizarAltura(NodoAVL nodo)
        {
            int izquierda = ObtenerAltura(nodo.Izquierda);
            int derecha = ObtenerAltura(nodo.Derecha);

            nodo.Altura = 1 + Math.Max(izquierda, derecha);
        }


        // --------------------------------------------------
        // CALCULAR BALANCE
        //
        // Altura izquierda menos altura derecha.
        // --------------------------------------------------

        private static int CalcularBalance(NodoAVL? nodo)
        {
            if (nodo == null)
                return 0;

            return ObtenerAltura(nodo.Izquierda)
                 - ObtenerAltura(nodo.Derecha);
        }


        // --------------------------------------------------
        // ROTACIÓN A LA DERECHA
        //
        // Se utiliza para corregir un desequilibrio LL.
        // --------------------------------------------------

        private static NodoAVL GirarDerecha(NodoAVL nodo)
        {
            if (nodo.Izquierda == null)
            {
                throw new InvalidOperationException(
                    "No se puede girar a la derecha.");
            }

            NodoAVL nuevoPadre = nodo.Izquierda;

            NodoAVL? temporal = nuevoPadre.Derecha;

            nuevoPadre.Derecha = nodo;

            nodo.Izquierda = temporal;

            ActualizarAltura(nodo);
            ActualizarAltura(nuevoPadre);

            return nuevoPadre;
        }


        // --------------------------------------------------
        // ROTACIÓN A LA IZQUIERDA
        //
        // Se utiliza para corregir un desequilibrio RR.
        // --------------------------------------------------

        private static NodoAVL GirarIzquierda(NodoAVL nodo)
        {
            if (nodo.Derecha == null)
            {
                throw new InvalidOperationException(
                    "No se puede girar a la izquierda.");
            }

            NodoAVL nuevoPadre = nodo.Derecha;

            NodoAVL? temporal = nuevoPadre.Izquierda;

            nuevoPadre.Izquierda = nodo;

            nodo.Derecha = temporal;

            ActualizarAltura(nodo);
            ActualizarAltura(nuevoPadre);

            return nuevoPadre;
        }


        // --------------------------------------------------
        // ROTACIÓN DOBLE IZQUIERDA-DERECHA
        //
        // Se utiliza para corregir un desequilibrio LR.
        // --------------------------------------------------

        private static NodoAVL GirarIzquierdaDerecha(
            NodoAVL nodo)
        {
            if (nodo.Izquierda == null)
            {
                throw new InvalidOperationException(
                    "No existe un hijo izquierdo.");
            }

            nodo.Izquierda = GirarIzquierda(nodo.Izquierda);

            return GirarDerecha(nodo);
        }


        // --------------------------------------------------
        // ROTACIÓN DOBLE DERECHA-IZQUIERDA
        //
        // Se utiliza para corregir un desequilibrio RL.
        // --------------------------------------------------

        private static NodoAVL GirarDerechaIzquierda(
            NodoAVL nodo)
        {
            if (nodo.Derecha == null)
            {
                throw new InvalidOperationException(
                    "No existe un hijo derecho.");
            }

            nodo.Derecha = GirarDerecha(nodo.Derecha);

            return GirarIzquierda(nodo);
        }


        // --------------------------------------------------
        // EQUILIBRAR EL ÁRBOL
        //
        // Revisamos el balance y corregimos cuando sea
        // necesario.
        // --------------------------------------------------

        private static NodoAVL Equilibrar(NodoAVL nodo)
        {
            ActualizarAltura(nodo);

            int balance = CalcularBalance(nodo);

            // El árbol tiene más altura a la izquierda.
            if (balance > 1)
            {
                if (CalcularBalance(nodo.Izquierda) < 0)
                {
                    return GirarIzquierdaDerecha(nodo);
                }

                return GirarDerecha(nodo);
            }

            // El árbol tiene más altura a la derecha.
            if (balance < -1)
            {
                if (CalcularBalance(nodo.Derecha) > 0)
                {
                    return GirarDerechaIzquierda(nodo);
                }

                return GirarIzquierda(nodo);
            }

            return nodo;
        }


        // ==================================================
        // AGREGAR PRODUCTO
        // ==================================================

        public bool AgregarProducto(Producto producto)
        {
            if (producto == null)
            {
                throw new ArgumentNullException(nameof(producto));
            }

            bool agregado = false;

            raiz = AgregarEnArbol(
                raiz,
                producto,
                ref agregado);

            if (agregado)
            {
                cantidadProductos++;
            }

            return agregado;
        }


        // Busca dónde colocar el nuevo producto.
        private static NodoAVL AgregarEnArbol(
            NodoAVL? nodo,
            Producto producto,
            ref bool agregado)
        {
            if (nodo == null)
            {
                agregado = true;

                return new NodoAVL(producto);
            }

            int resultado = comparador.Compare(
                producto.Codigo,
                nodo.Producto.Codigo);

            // El código es menor.
            if (resultado < 0)
            {
                nodo.Izquierda = AgregarEnArbol(
                    nodo.Izquierda,
                    producto,
                    ref agregado);
            }

            // El código es mayor.
            else if (resultado > 0)
            {
                nodo.Derecha = AgregarEnArbol(
                    nodo.Derecha,
                    producto,
                    ref agregado);
            }

            // El código ya existe.
            else
            {
                return nodo;
            }

            return Equilibrar(nodo);
        }


        // ==================================================
        // BUSCAR PRODUCTO
        // ==================================================

        public Producto? BuscarProducto(string codigo)
        {
            if (string.IsNullOrWhiteSpace(codigo))
                return null;

            NodoAVL? actual = raiz;

            while (actual != null)
            {
                int resultado = comparador.Compare(
                    codigo.Trim(),
                    actual.Producto.Codigo);

                if (resultado == 0)
                {
                    return actual.Producto;
                }

                if (resultado < 0)
                {
                    actual = actual.Izquierda;
                }
                else
                {
                    actual = actual.Derecha;
                }
            }

            return null;
        }


        // ==================================================
        // ACTUALIZAR PRODUCTO
        // ==================================================

        public bool ActualizarProducto(
            string codigo,
            string nombre,
            string categoria,
            double precio,
            int stock)
        {
            Producto? producto = BuscarProducto(codigo);

            if (producto == null)
            {
                return false;
            }

            producto.CambiarDatos(
                nombre,
                categoria,
                precio,
                stock);

            return true;
        }


        // ==================================================
        // ELIMINAR PRODUCTO
        // ==================================================

        public bool EliminarProducto(string codigo)
        {
            if (string.IsNullOrWhiteSpace(codigo))
                return false;

            bool eliminado = false;

            raiz = EliminarDelArbol(
                raiz,
                codigo.Trim(),
                ref eliminado);

            if (eliminado)
            {
                cantidadProductos--;
            }

            return eliminado;
        }


        // Elimina el producto según la cantidad de hijos.
        private static NodoAVL? EliminarDelArbol(
            NodoAVL? nodo,
            string codigo,
            ref bool eliminado)
        {
            if (nodo == null)
                return null;

            int resultado = comparador.Compare(
                codigo,
                nodo.Producto.Codigo);

            if (resultado < 0)
            {
                nodo.Izquierda = EliminarDelArbol(
                    nodo.Izquierda,
                    codigo,
                    ref eliminado);
            }
            else if (resultado > 0)
            {
                nodo.Derecha = EliminarDelArbol(
                    nodo.Derecha,
                    codigo,
                    ref eliminado);
            }
            else
            {
                eliminado = true;

                // CASO 1: el nodo no tiene hijos.
                if (nodo.Izquierda == null &&
                    nodo.Derecha == null)
                {
                    return null;
                }

                // CASO 2: tiene solamente un hijo.
                if (nodo.Izquierda == null)
                {
                    return nodo.Derecha;
                }

                if (nodo.Derecha == null)
                {
                    return nodo.Izquierda;
                }

                // CASO 3: el nodo tiene dos hijos.
                //
                // Buscamos el menor nodo del lado derecho.
                // Ese nodo ocupará el lugar del eliminado.

                NodoAVL siguiente;

                NodoAVL? nuevaDerecha = SacarMenor(
                    nodo.Derecha,
                    out siguiente);

                siguiente.Izquierda = nodo.Izquierda;
                siguiente.Derecha = nuevaDerecha;

                return Equilibrar(siguiente);
            }

            return Equilibrar(nodo);
        }


        // Retira el nodo con el código menor.
        // También equilibra los nodos afectados.
        private static NodoAVL? SacarMenor(
            NodoAVL nodo,
            out NodoAVL menor)
        {
            if (nodo.Izquierda == null)
            {
                menor = nodo;

                return nodo.Derecha;
            }

            nodo.Izquierda = SacarMenor(
                nodo.Izquierda,
                out menor);

            return Equilibrar(nodo);
        }


        // ==================================================
        // RECORRIDO IN-ORDEN
        //
        // Primero izquierda, luego nodo y después derecha.
        // ==================================================

        public List<Producto> VerProductosOrdenados()
        {
            List<Producto> lista = new List<Producto>();

            RecorrerArbol(raiz, lista);

            return lista;
        }


        private static void RecorrerArbol(
            NodoAVL? nodo,
            List<Producto> lista)
        {
            if (nodo == null)
                return;

            RecorrerArbol(nodo.Izquierda, lista);

            lista.Add(nodo.Producto);

            RecorrerArbol(nodo.Derecha, lista);
        }
    }


    // ======================================================
    // TIPOS DE CLIENTE
    //
    // El número menor tiene mayor prioridad.
    // ======================================================

    public enum TipoCliente
    {
        VIP = 1,
        ADULTO_MAYOR = 2,
        REGULAR = 3
    }


    // ======================================================
    // CLASE 4: CLIENTE
    // Guarda los datos de las personas que esperan atención.
    // ======================================================

    public class Cliente
    {
        public string Nombre { get; }

        public TipoCliente Tipo { get; }

        public int NumeroProductos { get; }

        public DateTime HoraLlegada { get; }

        // Número interno para respetar el orden de llegada.
        internal long NumeroTurno { get; set; }

        public Cliente(
            string nombre,
            TipoCliente tipo,
            int numeroProductos)
        {
            if (string.IsNullOrWhiteSpace(nombre))
            {
                throw new ArgumentException(
                    "Debe ingresar el nombre del cliente.");
            }

            if (!Enum.IsDefined(typeof(TipoCliente), tipo))
            {
                throw new ArgumentException(
                    "La categoría del cliente no es válida.");
            }

            if (numeroProductos < 0)
            {
                throw new ArgumentException(
                    "La cantidad de productos no puede ser negativa.");
            }

            Nombre = nombre.Trim();

            Tipo = tipo;

            NumeroProductos = numeroProductos;

            HoraLlegada = DateTime.Now;

            NumeroTurno = 0;
        }
    }


    // ======================================================
    // CLASE 5: COLA DE PRIORIDAD
    //
    // Organiza los clientes usando un Min Heap.
    //
    // VIP tiene prioridad 1.
    // Adulto mayor tiene prioridad 2.
    // Regular tiene prioridad 3.
    //
    // Cuando la prioridad es igual, se atiende a quien
    // llegó primero.
    // ======================================================

    public class ColaPrioridad
    {
        private readonly List<Cliente> clientes;

        private long ultimoTurno;

        public int CantidadClientes
        {
            get { return clientes.Count; }
        }

        public ColaPrioridad()
        {
            clientes = new List<Cliente>();

            ultimoTurno = 0;
        }


        // --------------------------------------------------
        // COMPARAR DOS CLIENTES
        // --------------------------------------------------

        private static int CompararClientes(
            Cliente primero,
            Cliente segundo)
        {
            int prioridad = ((int)primero.Tipo).CompareTo(
                (int)segundo.Tipo);

            // Primero revisamos la categoría.
            if (prioridad != 0)
            {
                return prioridad;
            }

            // Si tienen la misma categoría, respetamos FIFO.
            return primero.NumeroTurno.CompareTo(
                segundo.NumeroTurno);
        }


        // --------------------------------------------------
        // CAMBIAR DOS CLIENTES DE POSICIÓN
        // --------------------------------------------------

        private static void CambiarLugar(
            List<Cliente> lista,
            int primero,
            int segundo)
        {
            Cliente temporal = lista[primero];

            lista[primero] = lista[segundo];

            lista[segundo] = temporal;
        }


        // ==================================================
        // BUBBLE UP
        //
        // Cuando agregamos un cliente, lo comparamos con
        // su padre hasta encontrar su posición correcta.
        //
        // Complejidad O(log n).
        // ==================================================

        private static void SubirCliente(
            List<Cliente> lista,
            int posicion)
        {
            while (posicion > 0)
            {
                int padre = (posicion - 1) / 2;

                if (CompararClientes(
                    lista[posicion],
                    lista[padre]) >= 0)
                {
                    break;
                }

                CambiarLugar(lista, posicion, padre);

                posicion = padre;
            }
        }


        // ==================================================
        // BUBBLE DOWN
        //
        // Después de sacar al primer cliente, acomodamos
        // nuevamente el Heap.
        //
        // Complejidad O(log n).
        // ==================================================

        private static void BajarCliente(
            List<Cliente> lista,
            int posicion)
        {
            while (true)
            {
                int izquierdo = posicion * 2 + 1;

                int derecho = izquierdo + 1;

                int mejor = posicion;

                if (izquierdo < lista.Count &&
                    CompararClientes(
                        lista[izquierdo],
                        lista[mejor]) < 0)
                {
                    mejor = izquierdo;
                }

                if (derecho < lista.Count &&
                    CompararClientes(
                        lista[derecho],
                        lista[mejor]) < 0)
                {
                    mejor = derecho;
                }

                if (mejor == posicion)
                {
                    break;
                }

                CambiarLugar(lista, posicion, mejor);

                posicion = mejor;
            }
        }


        // ==================================================
        // AGREGAR CLIENTE A LA COLA
        // ==================================================

        public void AgregarCliente(Cliente cliente)
        {
            if (cliente == null)
            {
                throw new ArgumentNullException(nameof(cliente));
            }

            if (cliente.NumeroTurno != 0)
            {
                throw new InvalidOperationException(
                    "Este cliente ya fue registrado.");
            }

            if (ultimoTurno == long.MaxValue)
            {
                throw new InvalidOperationException(
                    "Se alcanzó el límite de turnos.");
            }

            ultimoTurno++;

            cliente.NumeroTurno = ultimoTurno;

            clientes.Add(cliente);

            // Acomodamos al nuevo cliente según su prioridad.
            SubirCliente(clientes, clientes.Count - 1);
        }


        // ==================================================
        // SACAR EL PRIMER CLIENTE
        //
        // Retira la raíz y restaura el Heap.
        // Complejidad O(log n).
        // ==================================================

        private static Cliente SacarPrimero(
            List<Cliente> lista)
        {
            Cliente primero = lista[0];

            int ultimo = lista.Count - 1;

            if (ultimo == 0)
            {
                lista.RemoveAt(0);

                return primero;
            }

            // Colocamos el último cliente en la raíz.
            lista[0] = lista[ultimo];

            lista.RemoveAt(ultimo);

            // Corregimos el orden.
            BajarCliente(lista, 0);

            return primero;
        }


        // ==================================================
        // ATENDER CLIENTE
        // ==================================================

        public Cliente? AtenderCliente()
        {
            if (clientes.Count == 0)
            {
                return null;
            }

            return SacarPrimero(clientes);
        }


        // ==================================================
        // CONSULTAR EL SIGUIENTE CLIENTE
        // ==================================================

        public Cliente? VerSiguienteCliente()
        {
            if (clientes.Count == 0)
            {
                return null;
            }

            return clientes[0];
        }


        // ==================================================
        // VER CLIENTES EN ORDEN DE ATENCIÓN
        //
        // Usamos una copia para no modificar la cola real.
        // ==================================================

        public List<Cliente> VerClientesEnOrden()
        {
            List<Cliente> copia =
                new List<Cliente>(clientes);

            List<Cliente> ordenados =
                new List<Cliente>();

            while (copia.Count > 0)
            {
                Cliente cliente = SacarPrimero(copia);

                ordenados.Add(cliente);
            }

            return ordenados;
        }


        // ==================================================
        // SABER EL TURNO DE UN CLIENTE
        // ==================================================

        public int BuscarPosicion(Cliente cliente)
        {
            List<Cliente> lista = VerClientesEnOrden();

            for (int i = 0; i < lista.Count; i++)
            {
                if (ReferenceEquals(lista[i], cliente))
                {
                    return i + 1;
                }
            }

            return -1;
        }
    }


    // ======================================================
    // CLASE 6: SUPERMERCADO
    //
    // Une el inventario y la atención de clientes.
    // ======================================================

    public class Supermercado
    {
        public ArbolAVL Inventario { get; }

        public ColaPrioridad Cola { get; }

        public int ClientesAtendidos { get; private set; }

        public Supermercado()
        {
            Inventario = new ArbolAVL();

            Cola = new ColaPrioridad();

            ClientesAtendidos = 0;
        }


        // Atiende al siguiente cliente.
        public Cliente? AtenderSiguiente()
        {
            Cliente? cliente = Cola.AtenderCliente();

            if (cliente != null)
            {
                ClientesAtendidos++;
            }

            return cliente;
        }


        // Suma todas las unidades del inventario.
        public long ContarUnidades()
        {
            long total = 0;

            List<Producto> productos =
                Inventario.VerProductosOrdenados();

            foreach (Producto producto in productos)
            {
                total += producto.Stock;
            }

            return total;
        }


        // Calcula el valor total de las existencias.
        public double CalcularValorInventario()
        {
            double total = 0;

            List<Producto> productos =
                Inventario.VerProductosOrdenados();

            foreach (Producto producto in productos)
            {
                total += producto.Precio * producto.Stock;
            }

            return total;
        }
    }


    // ======================================================
    // CLASE 7: MENU
    //
    // Muestra las opciones y recibe la información del usuario.
    // ======================================================

    public class Menu
    {
        private readonly Supermercado tienda;

        public Menu(Supermercado tienda)
        {
            this.tienda = tienda ??
                throw new ArgumentNullException(nameof(tienda));
        }


        // ==================================================
        // MOSTRAR MENÚ PRINCIPAL
        // ==================================================

        private void MostrarMenu()
        {
            Console.ForegroundColor = ConsoleColor.Yellow;

            Console.WriteLine();
            Console.WriteLine(
                "======================================================");

            Console.WriteLine(
                "                   MARKET DON JUAN");

            Console.WriteLine(
                "             SISTEMA DE GESTIÓN GENERAL");

            Console.WriteLine(
                "======================================================");

            Console.ResetColor();

            Console.WriteLine();
            Console.WriteLine(
                "---------------- INVENTARIO AVL ----------------------");

            Console.WriteLine("1. Registrar producto");
            Console.WriteLine("2. Buscar producto por código");
            Console.WriteLine("3. Actualizar producto");
            Console.WriteLine("4. Eliminar producto");
            Console.WriteLine("5. Mostrar inventario ordenado");

            Console.WriteLine();
            Console.WriteLine(
                "------------- ATENCIÓN DE CLIENTES -------------------");

            Console.WriteLine("6. Agregar cliente a la cola");
            Console.WriteLine("7. Atender siguiente cliente");
            Console.WriteLine("8. Mostrar clientes en espera");

            Console.WriteLine();
            Console.WriteLine(
                "------------------- REPORTES -------------------------");

            Console.WriteLine("9. Mostrar estadísticas del sistema");

            Console.WriteLine();
            Console.WriteLine(
                "------------------- SISTEMA --------------------------");

            Console.WriteLine("10. Salir");

            Console.WriteLine();
            Console.WriteLine(
                "======================================================");

            Console.Write("Seleccione una opción: ");
        }


        // ==================================================
        // PEDIR TEXTO
        //
        // No permite campos vacíos.
        // Escribir CANCELAR permite volver al menú.
        // ==================================================

        private static string PedirTexto(string mensaje)
        {
            while (true)
            {
                Console.Write(mensaje);

                string? texto = Console.ReadLine();

                if (texto == null)
                {
                    throw new OperationCanceledException();
                }

                texto = texto.Trim();

                if (texto.Equals(
                    "CANCELAR",
                    StringComparison.OrdinalIgnoreCase))
                {
                    throw new OperationCanceledException();
                }

                if (texto.Length > 0)
                {
                    return texto;
                }

                Console.WriteLine(
                    "Debe ingresar la información solicitada.");
            }
        }


        // ==================================================
        // PEDIR NÚMERO ENTERO
        // ==================================================

        private static int PedirNumero(
            string mensaje,
            int minimo)
        {
            while (true)
            {
                string texto = PedirTexto(mensaje);

                bool correcto = int.TryParse(
                    texto,
                    NumberStyles.Integer,
                    CultureInfo.InvariantCulture,
                    out int numero);

                if (correcto && numero >= minimo)
                {
                    return numero;
                }

                Console.WriteLine(
                    "Ingrese un número entero válido mayor o igual a "
                    + minimo + ".");
            }
        }


        // ==================================================
        // PEDIR PRECIO
        //
        // Acepta punto o coma como separador decimal.
        // ==================================================

        private static double PedirPrecio()
        {
            while (true)
            {
                string texto = PedirTexto(
                    "Precio del producto: Q");

                texto = texto.Replace(',', '.');

                bool correcto = double.TryParse(
                    texto,
                    NumberStyles.AllowDecimalPoint,
                    CultureInfo.InvariantCulture,
                    out double precio);

                if (correcto &&
                    precio > 0 &&
                    !double.IsNaN(precio) &&
                    !double.IsInfinity(precio))
                {
                    return precio;
                }

                Console.WriteLine(
                    "Ingrese un precio válido mayor que cero.");
            }
        }


        // ==================================================
        // MOSTRAR LOS DATOS DE UN PRODUCTO
        // ==================================================

        private static void MostrarProducto(Producto producto)
        {
            Console.WriteLine();
            Console.WriteLine("--------------------------------------");

            Console.WriteLine(
                "Código: " + producto.Codigo);

            Console.WriteLine(
                "Nombre: " + producto.Nombre);

            Console.WriteLine(
                "Categoría: " + producto.Categoria);

            Console.WriteLine(
                "Precio: Q" + producto.Precio.ToString("F2"));

            Console.WriteLine(
                "Stock: " + producto.Stock);

            Console.WriteLine("--------------------------------------");
        }


        // ==================================================
        // OPCIÓN 1: REGISTRAR PRODUCTO
        // ==================================================

        private void RegistrarProducto()
        {
            Console.WriteLine();
            Console.WriteLine("=== REGISTRAR PRODUCTO ===");
            Console.WriteLine();

            string codigo = PedirTexto(
                "Código del producto: ");

            // Revisamos si ya existe ese código.
            if (tienda.Inventario.BuscarProducto(codigo) != null)
            {
                Console.WriteLine();
                Console.WriteLine(
                    "Error: ya existe un producto con ese código.");

                return;
            }

            string nombre = PedirTexto(
                "Nombre del producto: ");

            string categoria = PedirTexto(
                "Categoría: ");

            double precio = PedirPrecio();

            int stock = PedirNumero(
                "Cantidad en stock: ",
                0);

            Producto nuevo = new Producto(
                codigo,
                nombre,
                categoria,
                precio,
                stock);

            bool resultado =
                tienda.Inventario.AgregarProducto(nuevo);

            if (resultado)
            {
                Console.ForegroundColor = ConsoleColor.Green;

                Console.WriteLine();
                Console.WriteLine(
                    "Producto registrado correctamente.");

                Console.ResetColor();

                MostrarProducto(nuevo);
            }
            else
            {
                Console.WriteLine(
                    "No se pudo registrar el producto.");
            }
        }


        // ==================================================
        // OPCIÓN 2: BUSCAR PRODUCTO
        // ==================================================

        private void BuscarProducto()
        {
            Console.WriteLine();
            Console.WriteLine("=== BUSCAR PRODUCTO ===");
            Console.WriteLine();

            string codigo = PedirTexto(
                "Ingrese el código del producto: ");

            Producto? encontrado =
                tienda.Inventario.BuscarProducto(codigo);

            if (encontrado == null)
            {
                Console.WriteLine();
                Console.WriteLine("Producto no encontrado.");

                return;
            }

            Console.WriteLine();
            Console.WriteLine("Producto encontrado:");

            MostrarProducto(encontrado);
        }


        // ==================================================
        // OPCIÓN 3: ACTUALIZAR PRODUCTO
        // ==================================================

        private void ActualizarProducto()
        {
            Console.WriteLine();
            Console.WriteLine("=== ACTUALIZAR PRODUCTO ===");
            Console.WriteLine();

            string codigo = PedirTexto(
                "Código del producto: ");

            Producto? producto =
                tienda.Inventario.BuscarProducto(codigo);

            if (producto == null)
            {
                Console.WriteLine();
                Console.WriteLine(
                    "No existe un producto con ese código.");

                return;
            }

            Console.WriteLine();
            Console.WriteLine("Datos actuales:");

            MostrarProducto(producto);

            Console.WriteLine();
            Console.WriteLine("Ingrese los nuevos datos.");
            Console.WriteLine(
                "El código del producto no puede modificarse.");

            Console.WriteLine();

            string nombre = PedirTexto(
                "Nuevo nombre: ");

            string categoria = PedirTexto(
                "Nueva categoría: ");

            double precio = PedirPrecio();

            int stock = PedirNumero(
                "Nuevo stock: ",
                0);

            bool resultado =
                tienda.Inventario.ActualizarProducto(
                    codigo,
                    nombre,
                    categoria,
                    precio,
                    stock);

            if (resultado)
            {
                Console.ForegroundColor = ConsoleColor.Green;

                Console.WriteLine();
                Console.WriteLine(
                    "Producto actualizado correctamente.");

                Console.ResetColor();

                Producto? actualizado =
                    tienda.Inventario.BuscarProducto(codigo);

                if (actualizado != null)
                {
                    MostrarProducto(actualizado);
                }
            }
        }


        // ==================================================
        // OPCIÓN 4: ELIMINAR PRODUCTO
        // ==================================================

        private void EliminarProducto()
        {
            Console.WriteLine();
            Console.WriteLine("=== ELIMINAR PRODUCTO ===");
            Console.WriteLine();

            string codigo = PedirTexto(
                "Código del producto: ");

            Producto? producto =
                tienda.Inventario.BuscarProducto(codigo);

            if (producto == null)
            {
                Console.WriteLine();
                Console.WriteLine(
                    "No se encontró el producto solicitado.");

                return;
            }

            MostrarProducto(producto);

            Console.WriteLine();

            string confirmar = PedirTexto(
                "¿Desea eliminar este producto? (S/N): ");

            if (!confirmar.Equals(
                "S",
                StringComparison.OrdinalIgnoreCase))
            {
                Console.WriteLine();
                Console.WriteLine("Eliminación cancelada.");

                return;
            }

            bool resultado =
                tienda.Inventario.EliminarProducto(codigo);

            if (resultado)
            {
                Console.ForegroundColor = ConsoleColor.Green;

                Console.WriteLine();
                Console.WriteLine(
                    "Producto eliminado correctamente.");

                Console.ResetColor();
            }
            else
            {
                Console.WriteLine(
                    "No fue posible eliminar el producto.");
            }
        }


        // ==================================================
        // OPCIÓN 5: MOSTRAR INVENTARIO
        // ==================================================

        private void MostrarInventario()
        {
            Console.WriteLine();
            Console.WriteLine("=== INVENTARIO MARKET DON JUAN ===");

            List<Producto> productos =
                tienda.Inventario.VerProductosOrdenados();

            if (productos.Count == 0)
            {
                Console.WriteLine();
                Console.WriteLine(
                    "Actualmente no hay productos registrados.");

                return;
            }

            Console.WriteLine();

            Console.WriteLine(
                "{0,-12} {1,-24} {2,-18} {3,10} {4,8}",
                "CÓDIGO",
                "NOMBRE",
                "CATEGORÍA",
                "PRECIO",
                "STOCK");

            Console.WriteLine(new string('-', 78));

            foreach (Producto producto in productos)
            {
                Console.WriteLine(
                    "{0,-12} {1,-24} {2,-18} {3,10:F2} {4,8}",
                    producto.Codigo,
                    producto.Nombre,
                    producto.Categoria,
                    producto.Precio,
                    producto.Stock);
            }

            Console.WriteLine(new string('-', 78));

            Console.WriteLine();
            Console.WriteLine(
                "Total de productos: " + productos.Count);
        }


        // ==================================================
        // OPCIÓN 6: AGREGAR CLIENTE
        // ==================================================

        private void AgregarCliente()
        {
            Console.WriteLine();
            Console.WriteLine("=== AGREGAR CLIENTE ===");
            Console.WriteLine();

            string nombre = PedirTexto(
                "Nombre del cliente: ");

            Console.WriteLine();
            Console.WriteLine("Seleccione el tipo de cliente:");

            Console.WriteLine("1. VIP");
            Console.WriteLine("2. Adulto mayor");
            Console.WriteLine("3. Regular");

            int numeroTipo;

            while (true)
            {
                numeroTipo = PedirNumero(
                    "Seleccione una opción: ",
                    1);

                if (numeroTipo >= 1 && numeroTipo <= 3)
                {
                    break;
                }

                Console.WriteLine(
                    "Seleccione un tipo entre 1 y 3.");
            }

            TipoCliente tipo =
                (TipoCliente)numeroTipo;

            int productos = PedirNumero(
                "Cantidad de productos que comprará: ",
                0);

            Cliente nuevo = new Cliente(
                nombre,
                tipo,
                productos);

            tienda.Cola.AgregarCliente(nuevo);

            int posicion =
                tienda.Cola.BuscarPosicion(nuevo);

            Console.ForegroundColor = ConsoleColor.Green;

            Console.WriteLine();
            Console.WriteLine(
                "Cliente agregado correctamente.");

            Console.ResetColor();

            Console.WriteLine();

            Console.WriteLine(
                "Nombre: " + nuevo.Nombre);

            Console.WriteLine(
                "Tipo: " + nuevo.Tipo);

            Console.WriteLine(
                "Productos: " + nuevo.NumeroProductos);

            Console.WriteLine(
                "Hora de llegada: " +
                nuevo.HoraLlegada.ToString("HH:mm:ss"));

            Console.WriteLine(
                "Posición estimada: " + posicion);
        }


        // ==================================================
        // OPCIÓN 7: ATENDER CLIENTE
        // ==================================================

        private void AtenderCliente()
        {
            Console.WriteLine();
            Console.WriteLine("=== ATENDER CLIENTE ===");

            Cliente? cliente = tienda.AtenderSiguiente();

            if (cliente == null)
            {
                Console.WriteLine();
                Console.WriteLine(
                    "No hay clientes esperando atención.");

                return;
            }

            Console.WriteLine();
            Console.WriteLine("Atendiendo al siguiente cliente:");

            Console.WriteLine();

            Console.WriteLine(
                "Nombre: " + cliente.Nombre);

            Console.WriteLine(
                "Tipo: " + cliente.Tipo);

            Console.WriteLine(
                "Productos: " + cliente.NumeroProductos);

            Console.WriteLine(
                "Hora de llegada: " +
                cliente.HoraLlegada.ToString("HH:mm:ss"));

            Console.ForegroundColor = ConsoleColor.Green;

            Console.WriteLine();
            Console.WriteLine(
                "Cliente atendido correctamente.");

            Console.ResetColor();
        }


        // ==================================================
        // OPCIÓN 8: MOSTRAR CLIENTES EN ESPERA
        // ==================================================

        private void MostrarClientes()
        {
            Console.WriteLine();
            Console.WriteLine("=== CLIENTES EN ESPERA ===");

            List<Cliente> lista =
                tienda.Cola.VerClientesEnOrden();

            if (lista.Count == 0)
            {
                Console.WriteLine();
                Console.WriteLine(
                    "No hay clientes registrados en la cola.");

                return;
            }

            Console.WriteLine();

            Console.WriteLine(
                "{0,-10} {1,-24} {2,-18} {3,-12} {4,-10}",
                "POSICIÓN",
                "NOMBRE",
                "TIPO",
                "PRODUCTOS",
                "HORA");

            Console.WriteLine(new string('-', 78));

            for (int i = 0; i < lista.Count; i++)
            {
                Cliente cliente = lista[i];

                Console.WriteLine(
                    "{0,-10} {1,-24} {2,-18} {3,-12} {4,-10}",
                    i + 1,
                    cliente.Nombre,
                    cliente.Tipo,
                    cliente.NumeroProductos,
                    cliente.HoraLlegada.ToString("HH:mm:ss"));
            }

            Console.WriteLine(new string('-', 78));

            Console.WriteLine();
            Console.WriteLine(
                "Total de clientes en espera: " + lista.Count);
        }


        // ==================================================
        // OPCIÓN 9: ESTADÍSTICAS
        // ==================================================

        private void MostrarEstadisticas()
        {
            Console.WriteLine();
            Console.WriteLine("=== ESTADÍSTICAS MARKET DON JUAN ===");

            Console.WriteLine();

            Console.WriteLine(
                "Productos registrados: " +
                tienda.Inventario.CantidadProductos);

            Console.WriteLine(
                "Unidades disponibles: " +
                tienda.ContarUnidades());

            Console.WriteLine(
                "Valor del inventario: Q" +
                tienda.CalcularValorInventario().ToString("F2"));

            Console.WriteLine(
                "Clientes esperando: " +
                tienda.Cola.CantidadClientes);

            Console.WriteLine(
                "Clientes atendidos: " +
                tienda.ClientesAtendidos);
        }


        // ==================================================
        // PAUSA
        // ==================================================

        private static void Pausar()
        {
            Console.WriteLine();
            Console.WriteLine(
                "Presione ENTER para regresar al menú.");

            Console.ReadLine();
        }


        // ==================================================
        // INICIAR SISTEMA
        //
        // El menú seguirá funcionando hasta elegir salir.
        // ==================================================

        public void Iniciar()
        {
            bool funcionando = true;

            while (funcionando)
            {
                Console.WriteLine();

                MostrarMenu();

                string? texto = Console.ReadLine();

                if (texto == null)
                {
                    break;
                }

                if (!int.TryParse(texto, out int opcion))
                {
                    Console.WriteLine();
                    Console.WriteLine(
                        "Debe ingresar un número válido.");

                    Pausar();
                    continue;
                }

                try
                {
                    switch (opcion)
                    {
                        case 1:
                            RegistrarProducto();
                            break;

                        case 2:
                            BuscarProducto();
                            break;

                        case 3:
                            ActualizarProducto();
                            break;

                        case 4:
                            EliminarProducto();
                            break;

                        case 5:
                            MostrarInventario();
                            break;

                        case 6:
                            AgregarCliente();
                            break;

                        case 7:
                            AtenderCliente();
                            break;

                        case 8:
                            MostrarClientes();
                            break;

                        case 9:
                            MostrarEstadisticas();
                            break;

                        case 10:
                            Console.WriteLine();
                            Console.WriteLine(
                                "Gracias por utilizar MARKET DON JUAN.");

                            Console.WriteLine(
                                "El programa ha finalizado.");

                            funcionando = false;
                            break;

                        default:
                            Console.WriteLine();
                            Console.WriteLine(
                                "Seleccione una opción entre 1 y 10.");
                            break;
                    }
                }
                catch (OperationCanceledException)
                {
                    Console.WriteLine();
                    Console.WriteLine(
                        "Operación cancelada. Volviendo al menú.");
                }
                catch (ArgumentException error)
                {
                    Console.ForegroundColor = ConsoleColor.Red;

                    Console.WriteLine();
                    Console.WriteLine(
                        "Error: " + error.Message);

                    Console.ResetColor();
                }
                catch (Exception error)
                {
                    Console.ForegroundColor = ConsoleColor.Red;

                    Console.WriteLine();
                    Console.WriteLine(
                        "Ocurrió un problema: " + error.Message);

                    Console.ResetColor();
                }

                if (funcionando)
                {
                    Pausar();
                }
            }
        }
    }


    // ======================================================
    // INICIO DEL PROGRAMA
    //
    // Aquí comienza la ejecución de Market Don Juan.
    // ======================================================

    internal static class Program
    {
        private static void Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;

            Supermercado tienda = new Supermercado();

            Menu menu = new Menu(tienda);

            menu.Iniciar();
        }
    }
}