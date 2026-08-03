import SvgIcon, { SvgIconProps } from "@mui/material/SvgIcon"

const CertificateIcon = (props: SvgIconProps) => {
	return (
		<SvgIcon {...props} viewBox="0 0 24 24">
			<path d="M19 3H5a2 2 0 0 0-2 2v14a2 2 0 0 0 2 2h7v-2H5V5h14v6h2V5a2 2 0 0 0-2-2z" />
			<path d="M8 7h8v2H8zM8 11h6v2H8z" />
			<path d="M16.5 13a3.5 3.5 0 1 0 0 7 3.5 3.5 0 0 0 0-7zm0 2a1.5 1.5 0 1 1 0 3 1.5 1.5 0 0 1 0-3z" />
			<path d="M14 17.8V23l2.5-1.5L19 23v-5.2a5.3 5.3 0 0 1-5 0z" />
		</SvgIcon>
	)
}

export default CertificateIcon
