'use client'
import { Registration } from "@/_page/auth";
import { Login } from "@/_page/auth";
import { Container } from "@mui/material";
import { createTheme, ThemeProvider } from "@mui/material/styles";
import { useState } from "react";

const base = createTheme()

declare module '@mui/material/styles' {
    interface PaletteColor {
        black?: PaletteOptions['primary'];
    }

    interface PaletteOptions {
        black?: PaletteOptions['primary'];
    }
}

const theme = createTheme({
    palette: {
        black: base.palette.augmentColor({
            color: { main: base.palette.grey[900] },
            name: "black",
        }),
        primary: { main: "#5b69e3" },
        secondary: {
            main: "#F3F3F3",
            light: "#FCFCFC",
            dark: "#EFEFEF"
        },
        mode: "light",
    },
})
const AuthPage = () => {

    const [focused, isFocused] = useState(false);

    return (
        <ThemeProvider theme={theme}>
            <Container
                className="flex items-stretch justify-between gap-6 p-0! w-full max-w-5xl bg-gray-100 rounded-3xl overflow-clip"
            >
                <Registration />
                <Login />
            </Container>
        </ThemeProvider>
    );
}

export default AuthPage;