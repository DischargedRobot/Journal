import SvgIcon, { SvgIconProps } from "@mui/material/SvgIcon"

const DepartmentsIcon = (props: SvgIconProps) => {

    return (
        <SvgIcon {...props} viewBox="0 0 64 64" sx={{ height: "64px", width: "64px" }}>
            <path d="M53.9727 18.9131L54.1836 19.0195H56.1904V19.79H7.80957V19.0195H9.81641L10.0273 18.9131L32 7.92676L53.9727 18.9131Z" fill="secondary.light" stroke="contrastingSecondary.main" strokeWidth="2" />
            <path d="M35 25V48H30V25H35Z" fill="secondary.light" stroke="contrastingSecondary.main" strokeWidth="2" />
            <path d="M49 25V48H44V25H49Z" fill="secondary.light" stroke="contrastingSecondary.main" strokeWidth="2" />
            <path d="M21 25V48H16V25H21Z" fill="secondary.light" stroke="contrastingSecondary.main" strokeWidth="2" />
            <path d="M53.4202 52.7695L53.4211 54.4834V55.4834H56.1917V56.1914H7.81079V55.4834H10.5813V52.5898L53.4202 52.7695Z" fill="secondary.light" stroke="contrastingSecondary.main" strokeWidth="2" />
        </SvgIcon>
    )
}

export default DepartmentsIcon