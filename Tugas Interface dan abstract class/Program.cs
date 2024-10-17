public abstract class Robot
{
    public string nama;
    public int energi;
    public int armor;
    public int serangan;

    protected Robot(string nama, int energi, int armor, int serangan)
    {
        this.nama = nama;
        this.energi = energi;
        this.armor = armor;
        this.serangan = serangan;
    }

    public virtual void Serang(Robot target)
    {
        int damage = Math.Max(0, serangan - target.armor);
        target.energi -= damage;
        Console.WriteLine($"{nama} menyerang {target.nama} dan menyebabkan {damage} kerusakan!");
    }

    public abstract void GunakanKemampuan(IKemampuan kemampuan, Robot target);

    public virtual void CetakInformasi()
    {
        Console.WriteLine($"Nama: {nama}, Energi: {energi}, Armor: {armor}, Serangan: {serangan}");
    }

    public virtual void PulihkanEnergi(int jumlah)
    {
        energi += jumlah;
        Console.WriteLine($"{nama} memulihkan {jumlah} energi.");
    }
}

public interface IKemampuan
{
    string Nama { get; }
    void Gunakan(Robot pengguna, Robot target);
    bool DapatDigunakan();
    void ResetCooldown();
}

public class Perbaikan : IKemampuan
{
    public string Nama => "Perbaikan";
    private int cooldown = 0;

    public void Gunakan(Robot pengguna, Robot target)
    {
        int jumlahPerbaikan = 20;
        pengguna.PulihkanEnergi(jumlahPerbaikan);
        cooldown = 3;
    }

    public bool DapatDigunakan() => cooldown == 0;

    public void ResetCooldown()
    {
        if (cooldown > 0) cooldown--;
    }
}

public class SeranganListrik : IKemampuan
{
    public string Nama => "Serangan Listrik";
    private int cooldown = 0;

    public void Gunakan(Robot pengguna, Robot target)
    {
        int damage = 15;
        target.energi -= damage;
        Console.WriteLine($"{pengguna.nama} menggunakan Serangan Listrik pada {target.nama}, menyebabkan {damage} kerusakan!");
        cooldown = 2;
    }

    public bool DapatDigunakan() => cooldown == 0;

    public void ResetCooldown()
    {
        if (cooldown > 0) cooldown--;
    }
}

public class SeranganPlasma : IKemampuan
{
    public string Nama => "Serangan Plasma";
    private int cooldown = 0;

    public void Gunakan(Robot pengguna, Robot target)
    {
        int damage = 25;
        if (target is BosRobot bosRobot)
        {
            bosRobot.Diserang(pengguna, true);
        }
        else
        {
            target.energi -= damage;
        }
        Console.WriteLine($"{pengguna.nama} menggunakan Serangan Plasma pada {target.nama}, menyebabkan {damage} kerusakan yang menembus armor!");
        cooldown = 4;
    }

    public bool DapatDigunakan() => cooldown == 0;

    public void ResetCooldown()
    {
        if (cooldown > 0) cooldown--;
    }
}

public class PertahananSuper : IKemampuan
{
    public string Nama => "Pertahanan Super";
    private int cooldown = 0;

    public void Gunakan(Robot pengguna, Robot target)
    {
        pengguna.armor += 10;
        Console.WriteLine($"{pengguna.nama} mengaktifkan Pertahanan Super, meningkatkan armor sebesar 10!");
        cooldown = 3;
    }

    public bool DapatDigunakan() => cooldown == 0;

    public void ResetCooldown()
    {
        if (cooldown > 0) cooldown--;
    }
}

public class RobotBiasa : Robot
{
    private List<IKemampuan> kemampuan;

    public RobotBiasa(string nama, int energi, int armor, int serangan) : base(nama, energi, armor, serangan)
    {
        kemampuan = new List<IKemampuan>
        {
            new Perbaikan(),
            new SeranganListrik()
        };
    }

    public override void GunakanKemampuan(IKemampuan kemampuan, Robot target)
    {
        if (kemampuan.DapatDigunakan())
        {
            kemampuan.Gunakan(this, target);
        }
        else
        {
            Console.WriteLine($"{nama} tidak dapat menggunakan {kemampuan.Nama} karena masih dalam cooldown.");
        }
    }

    public List<IKemampuan> DapatkanKemampuan()
    {
        return kemampuan;
    }
}

public class RobotKhusus : Robot
{
    private List<IKemampuan> kemampuan;

    public RobotKhusus(string nama, int energi, int armor, int serangan) : base(nama, energi, armor, serangan)
    {
        kemampuan = new List<IKemampuan>
        {
            new SeranganPlasma(),
            new PertahananSuper()
        };
    }

    public override void GunakanKemampuan(IKemampuan kemampuan, Robot target)
    {
        if (kemampuan.DapatDigunakan())
        {
            kemampuan.Gunakan(this, target);
        }
        else
        {
            Console.WriteLine($"{nama} tidak dapat menggunakan {kemampuan.Nama} karena masih dalam cooldown.");
        }
    }

    public List<IKemampuan> DapatkanKemampuan()
    {
        return kemampuan;
    }
}

public class BosRobot : Robot
{
    public int pertahanan;

    public BosRobot(string nama, int energi, int pertahanan, int serangan) : base(nama, energi, pertahanan / 2, serangan)
    {
        this.pertahanan = pertahanan;
    }

    public void Diserang(Robot penyerang, bool ignoreArmor = false)
    {
        int damage;
        if (ignoreArmor)
        {
            damage = penyerang.serangan;
        }
        else
        {
            damage = Math.Max(0, penyerang.serangan - pertahanan);
        }
        energi -= damage;
        Console.WriteLine($"{nama} menerima serangan dari {penyerang.nama} dan menerima {damage} kerusakan!");
    }

    public void Mati()
    {
        Console.WriteLine($"{nama} telah dikalahkan!");
    }

    public override void GunakanKemampuan(IKemampuan kemampuan, Robot target)
    {
        Console.WriteLine($"{nama} tidak memiliki kemampuan khusus.");
    }
}

public class Game
{
    private List<Robot> robots;
    private BosRobot bosRobot;

    public Game()
    {
        robots = new List<Robot>
        {
            new RobotBiasa("R2D2", 130, 50, 18),
            new RobotKhusus("C3PO", 150, 45, 122)
        };
        bosRobot = new BosRobot("Megatron", 200, 15, 25);
    }

    public void JalankanSimulasi()
    {
        Console.WriteLine("=====================================================");
        Console.WriteLine("Selamat datang di Simulator Pertarungan Antar Robot!");
        Console.WriteLine("Pertarungan dimulai!");
        Console.WriteLine("=====================================================\n");

        int giliran = 1;
        while (bosRobot.energi > 0 && robots.Exists(r => r.energi > 0))
        {
            Console.WriteLine($"=== Giliran {giliran} ===");

            foreach (var robot in robots)
            {
                if (robot.energi <= 0) continue;

                Console.WriteLine($"\n{robot.nama} giliran:");
                robot.CetakInformasi();

                Console.WriteLine("\nPilih aksi:");
                Console.WriteLine("1. Serang Bos Robot");
                Console.WriteLine("2. Gunakan Kemampuan");

                int pilihan;
                while (!int.TryParse(Console.ReadLine(), out pilihan) || pilihan < 1 || pilihan > 2)
                {
                    Console.WriteLine("Pilihan tidak valid. Silakan pilih 1 atau 2.");
                }

                if (pilihan == 1)
                {
                    robot.Serang(bosRobot);
                }
                else
                {
                    List<IKemampuan> kemampuanList = (robot is RobotBiasa) ? ((RobotBiasa)robot).DapatkanKemampuan() : ((RobotKhusus)robot).DapatkanKemampuan();
                    Console.WriteLine("\nPilih kemampuan:");
                    for (int i = 0; i < kemampuanList.Count; i++)
                    {
                        Console.WriteLine($"{i + 1}. {kemampuanList[i].Nama}");
                    }

                    int pilihanKemampuan;
                    while (!int.TryParse(Console.ReadLine(), out pilihanKemampuan) || pilihanKemampuan < 1 || pilihanKemampuan > kemampuanList.Count)
                    {
                        Console.WriteLine($"Pilihan tidak valid. Silakan pilih 1 sampai {kemampuanList.Count}.");
                    }

                    robot.GunakanKemampuan(kemampuanList[pilihanKemampuan - 1], bosRobot);
                }

                if (bosRobot.energi > 0)
                {
                    robot.energi -= Math.Max(0, bosRobot.serangan - robot.armor);
                    Console.WriteLine($"{bosRobot.nama} menyerang balik {robot.nama} dan menyebabkan {Math.Max(0, bosRobot.serangan - robot.armor)} kerusakan!");
                }

                robot.PulihkanEnergi(5);

                if (robot is RobotBiasa robotBiasa)
                {
                    foreach (var kemampuan in robotBiasa.DapatkanKemampuan())
                    {
                        kemampuan.ResetCooldown();
                    }
                }
                else if (robot is RobotKhusus robotKhusus)
                {
                    foreach (var kemampuan in robotKhusus.DapatkanKemampuan())
                    {
                        kemampuan.ResetCooldown();
                    }
                }
            }

            Console.WriteLine("\nStatus Bos Robot:");
            bosRobot.CetakInformasi();

            giliran++;
            Console.WriteLine("\nTekan Enter untuk melanjutkan");
            Console.ReadLine();
        }

        if (bosRobot.energi <= 0)
        {
            Console.WriteLine("Selamat! Para robot berhasil mengalahkan Bos Robot!");
        }
        else
        {
            Console.WriteLine("Game Over! Bos Robot berhasil mengalahkan semua robot.");
        }
    }
}

class Program
{
    static void Main(string[] args)
    {
        Game game = new Game();
        game.JalankanSimulasi();
    }
}

