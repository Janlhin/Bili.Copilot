//! BBDown 兼容层：把哔哩助理生成的 BBDown 命令翻译为 bili-suite CLI。
//!
//! 目的：在不改动 WinUI 调用方的前提下，把内置 BBDown.exe 换成 Janlhin/bili-suite
//! 的 `bili.exe dl`。本程序只做参数翻译与进程转发，不重新实现下载逻辑。

use std::env;
use std::ffi::OsString;
use std::path::{Path, PathBuf};
use std::process::{Command, ExitCode};

const CHILD: &str = "bili.exe";

#[derive(Default)]
struct Options {
    codec: Option<String>,
    output: Option<PathBuf>,
    max_height: Option<u32>,
    danmaku: bool,
    surface: Option<String>,
    ffmpeg_dir: Option<PathBuf>,
}

fn warn(msg: &str) {
    eprintln!("[bbdown-shim] {msg}");
}

/// BBDown 的 qn 码与分辨率并非线性关系，这里只映射常见值。
fn quality_to_height(raw: &str) -> Option<u32> {
    let upper = raw.to_ascii_uppercase();
    if upper.contains("8K") {
        return Some(4320);
    }
    if upper.contains("4K") {
        return Some(2160);
    }
    let digits: String = raw.chars().filter(|c| c.is_ascii_digit()).collect();
    let n: u32 = digits.parse().ok()?;
    if n >= 100 {
        return Some(n);
    }
    Some(match n {
        120 => 2160,
        116 | 112 | 80 => 1080,
        74 | 64 => 720,
        32 => 480,
        16 => 360,
        6 => 240,
        _ => return None,
    })
}

fn translate(args: &[String]) -> Result<Option<(String, Options)>, String> {
    let mut opts = Options::default();
    let mut inputs: Vec<String> = Vec::new();
    let mut i = 0;

    while i < args.len() {
        let arg = args[i].as_str();
        match arg {
            "-e" | "--encoding" => {
                i += 1;
                opts.codec = args.get(i).cloned();
            }
            "--work-dir" => {
                i += 1;
                opts.output = args.get(i).map(PathBuf::from);
            }
            "-ua" | "--cookie" | "-token" | "--skip-ai" => {
                i += 1; // bili-suite 使用自己的凭据库，忽略 Bili.Copilot 传入的凭据。
            }
            "--ffmpeg-path" => {
                i += 1;
                opts.ffmpeg_dir = args.get(i).map(PathBuf::from);
            }
            "-q" | "--quality" => {
                i += 1;
                opts.max_height = args.get(i).and_then(|v| quality_to_height(v));
            }
            "-dd" => opts.danmaku = true,
            "-app" => opts.surface = Some("app".into()),
            "--select-page" => {
                i += 1;
                warn("--select-page 尚未映射到 bili-suite，已忽略；请到下载器内选择分 P。");
            }
            "--cover-only" | "--danmaku-only" | "--audio-only" | "--sub-only" => {
                return Err(format!("{arg} 暂未由 bili-suite 支持，本次未下载。"));
            }
            "--help" | "-h" => {
                println!("BBDown -> bili-suite 兼容层。用法与 BBDown 相同，参数会被翻译后转发给 bili.exe dl。");
                return Ok(None);
            }
            "--version" => {
                println!("bbdown-shim 0.1.0 (bili-suite backend)");
                return Ok(None);
            }
            other if other.starts_with('-') => warn(&format!("未知参数 {other}，已忽略。")),
            other => inputs.push(other.to_string()),
        }
        i += 1;
    }

    if inputs.is_empty() {
        return Err("缺少视频 URL / BV / av / ep / ss。".into());
    }

    warn("Bili.Copilot 的 Cookie/Token 已忽略：首次使用请先执行 `bili.exe login` 完成扫码登录。");
    Ok(Some((inputs.remove(0), opts)))
}

fn locate_child() -> Result<PathBuf, String> {
    let exe = env::current_exe().map_err(|e| format!("读取当前进程路径失败: {e}"))?;
    let sibling = exe.parent().unwrap_or(Path::new("")).join(CHILD);
    if sibling.is_file() {
        return Ok(sibling);
    }
    Ok(PathBuf::from(CHILD))
}

fn main() -> ExitCode {
    let args: Vec<String> = env::args().skip(1).collect();
    let (input, opts) = match translate(&args) {
        Ok(Some(v)) => v,
        Ok(None) => return ExitCode::SUCCESS,
        Err(msg) => {
            warn(&msg);
            return ExitCode::from(2);
        }
    };

    if let Some(dir) = opts.ffmpeg_dir {
        if dir.is_dir() {
            let mut path = OsString::from(dir);
            path.push(";");
            path.push(env::var_os("PATH").unwrap_or_default());
            env::set_var("PATH", path);
        }
    }

    let mut cmd = Command::new(locate_child().unwrap_or_else(|_| PathBuf::from(CHILD)));
    cmd.arg("dl").arg(&input);
    if let Some(codec) = opts.codec {
        cmd.arg("--codec").arg(codec);
    }
    if let Some(dir) = opts.output {
        cmd.arg("--output").arg(dir);
    }
    if let Some(height) = opts.max_height {
        cmd.arg("--max-height").arg(height.to_string());
    }
    if opts.danmaku {
        cmd.arg("--danmaku").arg("both");
    }
    if let Some(surface) = opts.surface {
        cmd.arg("--surface").arg(surface);
    }

    match cmd.status() {
        Ok(status) => ExitCode::from(status.code().unwrap_or(1) as u8),
        Err(e) => {
            warn(&format!("启动 bili-suite 失败: {e}"));
            ExitCode::from(2)
        }
    }
}
